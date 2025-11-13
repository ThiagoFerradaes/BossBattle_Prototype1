using System;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CyrusShieldAttackManager : SkillObjectManager {
    // Components
    CyrusShieldSkillSO _info;

    // Atributes
    int _skillLevel = 0;
    bool hasDebuffed, hasReShielded;

    Action _onShieldBreak;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.SpearAttackTriggerName, _info.AnimationName, 0));
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusShieldSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        _onShieldBreak = ShieldBreak;

        healthManager.OnShieldBreak += _onShieldBreak;
    }

    private void OnDestroy() {
        healthManager.OnShieldBreak -= _onShieldBreak;
    }
    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
        if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
    }
    public override void ThirdFunc() {
        base.ThirdFunc();

        ShieldUp();
    }

    public override void FourthFunc() {
        base.FourthFunc();

        UnblockInputs();
    }

    void ShieldUp() {
        float shield = _skillLevel < 2 ? _info.ShieldAmount : _info.ShieldAmountLevelTwo;
        float shieldDuration = _skillLevel < 2 ? _info.ShieldDuration : _info.ShieldDurationLevelTwo;

        healthManager.RecieveShield(shield, shieldDuration);

        Buff();
    }

    void Buff() {

        if (_skillLevel < 1) return;

        hasDebuffed = false;

        statusManager.ChangeStatus(StatusType.BaseAttack, _info.IncreaseInAbyssalDamage / 100, true);
        statusManager.ChangeStatus(StatusType.SkillAttack, _info.IncreaseInAncestralDamage / 100, true);
        statusManager.ChangeStatus(StatusType.AttackSpeed, _info.IncreaseInAttackSpeed / 100, true);
    }

    void BuffOff() {

        if (_skillLevel < 1 || hasDebuffed) return;

        hasDebuffed = true;

        statusManager.ChangeStatus(StatusType.BaseAttack, _info.IncreaseInAbyssalDamage / 100, false);
        statusManager.ChangeStatus(StatusType.SkillAttack, _info.IncreaseInAncestralDamage / 100, false);
        statusManager.ChangeStatus(StatusType.AttackSpeed, _info.IncreaseInAttackSpeed / 100, false);
    }
    void ShieldBreak() {
        SkillAnimationEvent animationEvent = _info.Prefabs[0][0];
        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(animationEvent.PreFab, TypeOfSkillPrefab.Hitbox);
        Vector3 size = _skillLevel < 2 ? _info.SkillDamageAtributes.Size : _info.ShieldExplosionSizeLevelTwo * Vector3.one;

        prefab.transform.localScale = size;

        prefab.transform.SetParent(parent.transform, false);
        prefab.transform.SetLocalPositionAndRotation(animationEvent.PreFabPosition, Quaternion.identity);

        DamageContext newContext = new(_info.SkillDamageAtributes, statusManager);

        InstantDamageHitBox hitbox = prefab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        BuffOff();

        ReShield();
    }

    void ReShield() {
        if (_skillLevel < 3 || hasReShielded) End();
        else {
            ShieldUp();
            hasReShielded = true;
        }
    }
    public override void End() {

        healthManager.OnShieldBreak -= _onShieldBreak;

        hasReShielded = false;

        base.End();
    }
}
