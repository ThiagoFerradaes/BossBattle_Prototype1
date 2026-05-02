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
    Action<int, HeatArea> _onShootAction;
    Action<HeatArea> _onIgnisAction;

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

        _onShootAction = (int attackIdex, HeatArea area) => _attackCoroutine ??= StartCoroutine(SecondaryShoot(attackIdex, area));
        _onIgnisAction = (HeatArea area) => _ignisCoroutine ??= StartCoroutine(SecondaryIgnis(area));
    }

    private void OnDestroy() {
        BastianBaseAttackManager.OnShoot -= _onShootAction;
        //BastianIgnisManager.OnIgnisShoot -= _onIgnisAction;
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
        //BastianIgnisManager.OnIgnisShoot += _onIgnisAction;

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

    IEnumerator SecondaryShoot(int attackIndex, HeatArea area)
    {
        float realTimer = _info.TimeBetweenFirstAndSecondShoot / _statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        yield return new WaitForSeconds(realTimer);

        var prefabList = _info.Prefabs[attackIndex];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < prefabList.Count; i++)
        {
            SkillAnimationEvent prefabInfo = prefabList[i];

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateSecondShoot(prefabInfo, attackIndex, area);
            else if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(prefabInfo);
        }

        _attackCoroutine = null;
    }

    IEnumerator SecondaryIgnis(HeatArea area) {
        float realTimer = _info.TimeBetweenIgnis / _statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        yield return new WaitForSeconds(realTimer);

        var prefabList = _info.Prefabs[3];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < prefabList.Count; i++) {
            SkillAnimationEvent prefabInfo = prefabList[i];

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateIgnis(prefabInfo, area);
            else if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(prefabInfo);
        }

        _ignisCoroutine = null;
    }

    public override void EndWithUnblockSkills()
    {
        BastianBaseAttackManager.OnShoot -= _onShootAction;
        BastianIgnisManager.OnIgnisShoot -= _onIgnisAction;

        _energyManager.SetCanGainEnergy(true);

        base.EndWithUnblockSkills();
    }

    void InstantiateSecondShoot(SkillAnimationEvent prefabInfo, int attackIndex, HeatArea area) {
        DamageAtributes atributes = attackIndex switch {
            1 => _info.FirstAttackDamageAtributes,
            2 => _info.SecondAttackDamageAtributes,
            3 => _info.ThirdAttackDamageAtributes,
            _ => _info.FirstAttackDamageAtributes,
        };

        float pen = area >= HeatArea.SuperHeatArea ? _info.SPenetrationOnSuperHeat : 0;
        float critChance = area >= HeatArea.OverHeatArea ? _info.SCritChanceOverHeat : 0;
        float additionalCriDmg = area >= HeatArea.ExtremeHeatArea ? _info.SLastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        DamageAtributes newAtributes = new(atributes);

        newAtributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = critChance;
        newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.ProjectileSize * Vector3.one;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        DamageContext newContext = new(
            newAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        
        hitbox.Initialize(newContext);

        if (area < HeatArea.OverHeatArea) {
            BastianPassiveManager.Instance.GainHeat(_info.SHeatGain);
        }
        else BastianPassiveManager.Instance.GainHeat(_info.SHeatGainOverHeat);
    }

    void InstantiateIgnis(SkillAnimationEvent prefabInfo, HeatArea area) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.IgnisDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = area >= HeatArea.SuperHeatArea ? _info.IgnisPenetrationOnSuperHeat : 0;
        float critChance = area >= HeatArea.OverHeatArea ? _info.IgnisCritChanceOverHeat : 0;
        float additionalCriDmg = area >= HeatArea.ExtremeHeatArea ? _info.IgnisLastOverHeatCritDamage : 0;
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
        else BastianPassiveManager.Instance.GainHeat(_info.IgnisHeatGainOverHeat);
    }
}
