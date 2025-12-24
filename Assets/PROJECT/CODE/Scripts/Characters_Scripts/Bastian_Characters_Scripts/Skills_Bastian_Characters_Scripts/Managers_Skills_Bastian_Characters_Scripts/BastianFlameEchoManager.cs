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

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 1));

        _onShootAction = (int attackIdex) => StartCoroutine(SecondaryShoot(attackIdex));
    }

    private void OnDestroy() {
        BastianBaseAttackManager.OnShoot -= _onShootAction;
    }

    public override void FirstFunc() {
        base.FirstFunc();
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
        _energyManager.LooseAllEnergy();
    }

    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    public override void FourthFunc() {
        animationCoroutine = null;

        // Avisando que não ta mais em animação
        skillManager.SkillIsInAnimation(false);

        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Desbloqueando inputs
        UnblockInputs();

        StartCoroutine(Duration());

        BastianBaseAttackManager.OnShoot += _onShootAction;
    }

    IEnumerator Duration()
    {
        _energyManager.SetCanGainEnergy(false);
        yield return new WaitForSeconds(_info.UltimateDuration);

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
        }
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

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.SPenetrationOnSuperHeat : 0;
        //float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.SCritChanceOverHeat : 0;
        //float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.SLastOverHeatCritDamage : 0;
        //float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        newAtributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;

        DamageContext newContext = new(
            newAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);
    }
}
