using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianFlameEchoManager : SkillObjectManager
{
    // Components
    BastianFlameEchoSO _info;
    EnergyManager _energyManager;
    StatusManager _statusManager;

    // Actions
    Action<int> _onShootAction;
    Action _onIgnisAction;

    // Coroutines
    Coroutine _ignisCoroutine, _attackCoroutine;

    float _attackSpeedMultiplier;
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx)
    {
        if (!BastianPassiveManager.Instance.CanShoot)
        {
            return;
        }

        base.HandleInput(skill, ctx);
    }
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        if (_info == null) _info = skill as BastianFlameEchoSO;
        if (_energyManager == null) _energyManager = parent.GetComponent<EnergyManager>();
        if (_statusManager == null) _statusManager = parent.gameObject.GetComponent<StatusManager>();

        gameObject.SetActive(true);

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, 1, _attackSpeedMultiplier));

        _onShootAction = (int attackIdex) => _attackCoroutine ??= StartCoroutine(SecondaryShoot(attackIdex));
        _onIgnisAction = () => _ignisCoroutine ??= StartCoroutine(SecondaryIgnis());
    }

    private void OnDestroy() {
        BastianBaseAttackManager.OnShoot -= _onShootAction;
    }

    protected override void FirstFunc() {
        base.FirstFunc();

        _energyManager.LooseAllEnergy();

        _info.SkillSound.Post(parent);
    }

    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    protected override void FourthFunc() {
        animationCoroutine = null;

        // Avisando que n�o ta mais em anima��o
        skillManager.SkillIsInAnimation(false);

        // Desbloqueando inputs
        UnblockInputs();

        BastianBaseAttackManager.OnShoot += _onShootAction;
        BastianIgnisManager.OnIgnisShoot += _onIgnisAction;

        StartCoroutine(Duration());
    }

    IEnumerator Duration()
    {
        _energyManager.SetCanGainEnergy(false);
        yield return new WaitForSeconds(_info.UltimateDuration);

        BastianBaseAttackManager.OnShoot -= _onShootAction;
        BastianIgnisManager.OnIgnisShoot -= _onIgnisAction;

        EndWithUnblockSkills();
    }

    IEnumerator SecondaryShoot(int attackIndex)
    {
        float realTimer = _info.TimeBetweenFirstAndSecondShoot / _statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        yield return new WaitForSeconds(realTimer);

        var prefabList = _info.Prefabs[attackIndex];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < prefabList.Count; i++)
        {
            SkillAnimationEvent prefabInfo = prefabList[i];

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateSecondShoot(prefabInfo, attackIndex);
            else if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(prefabInfo);
        }

        _attackCoroutine = null;
    }

    IEnumerator SecondaryIgnis() {
        float realTimer = _info.TimeBetweenIgnis / _statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        yield return new WaitForSeconds(realTimer);

        var prefabList = _info.Prefabs[3];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < prefabList.Count; i++) {
            SkillAnimationEvent prefabInfo = prefabList[i];

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateIgnis(prefabInfo);
            else if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(prefabInfo);
        }

        _ignisCoroutine = null;
    }

    public override void EndWithUnblockSkills()
    {
        BastianBaseAttackManager.OnShoot -= _onShootAction;

        _energyManager.SetCanGainEnergy(true);

        base.EndWithUnblockSkills();
    }

    void InstantiateSecondShoot(SkillAnimationEvent prefabInfo, int attackIndex) {
        DamageAtributes atributes = attackIndex switch {
            1 => _info.FirstAttackDamageAtributes,
            2 => _info.SecondAttackDamageAtributes,
            3 => _info.ThirdAttackDamageAtributes,
            _ => _info.FirstAttackDamageAtributes,
        };

        DamageAtributes newAtributes = new(atributes);

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.ProjectileSize * Vector3.one;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        DamageContext newContext = new(
            newAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);
    }

    void InstantiateIgnis(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.IgnisDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.IgnisPenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.IgnisCritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.ExtremeHeatArea) ? _info.IgnisLastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        DamageAtributes atributes = new(_info.IgnisDamageAtributes);
        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = critChance;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        if (BastianPassiveManager.Instance.ReturnMaxHeat(HeatArea.SuperHeatArea))
            BastianPassiveManager.Instance.GainHeat(_info.IgnisHeatGain);
        else BastianPassiveManager.Instance.GainHeat(1);
    }
}
