using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianIgnisManager : SkillObjectManager {
    // Components
    BastianIgnisSO _info;

    float _attackSpeedMultiplier;

    public static event Action<BastianHeatArea> OnIgnisShoot;

    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx)
    {
        if (!BastianPassiveManager.Instance.CanShoot)
        {
            return;
        }

        base.HandleInput(skill, ctx);
    }

    public override void UseSkill(SkillSO skill) {


        if (_info == null) _info = skill as BastianIgnisSO;

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0,0, _attackSpeedMultiplier));
    }

    protected override void FirstFunc() {
        cooldownManager.SetCooldownWithCharges(slot, _info);

        skillManager.SkillIsInAnimation(true);
    }

    protected override void FourthFunc() {
        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        EndWithUnblockSkills();
    }
    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.SkillDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        DamageAtributes atributes = _info.SkillDamageAtributes;
        
        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };


        OnIgnisShoot?.Invoke(BastianPassiveManager.Instance.ReturnCurrentHeatArea());


        _info.SkillSound.Post(parent);

        BastianPassiveManager.Instance.GainHeat(_info.HeatGain);

    }
}
