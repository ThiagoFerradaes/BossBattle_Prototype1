using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class BastianFlameEchoManager : SkillObjectManager
{
    // Components
    BastianFlameEchoSO _info;
    EnergyManager _energyManager;
    StatusManager _statusManager;

    // Actions
    Action<int> _onShootAction;
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        if (_info == null) _info = skill as BastianFlameEchoSO;
        if (_energyManager == null) _energyManager = parent.GetComponent<EnergyManager>();
        if (_statusManager == null) _statusManager = parent.gameObject.GetComponent<StatusManager>();

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(Attack());

        _onShootAction = (int attackIdex) => StartCoroutine(SecondaryShoot(attackIdex));
    }

    IEnumerator Attack()
    {
        skillManager.SkillIsInAnimation(true);

        // Animation
        anim.SetTrigger(_info.AnimationParameter);
        AnimatorStateInfo stateInfo;

        yield return null;

        int layer = 0; // Encontrando a layer da animação
        for (int i = 0; i < anim.layerCount; i++)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(i);

            if (state.IsName(_info.AnimationName) || nextState.IsName(_info.AnimationName))
            {
                layer = i;
                break;
            }
        }

        do
        { // Esperando entrar na animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
        } while (!stateInfo.IsName(_info.AnimationName));

        // Perdendo energia
        _energyManager.LooseAllEnergy();

        int attackStateHash = stateInfo.fullPathHash;

        var prefabList = _info.Prefabs[1];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        // Instanciando prefabs
        for (int i = 0; i < prefabList.Count; i++)
        {
            SkillAnimationEvent prefabInfo = prefabList[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do
            { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX)
            {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash)
        {
            yield return null;
        }

        // Corrotina
        animationCoroutine = null;

        // Avisando que não ta mais em animação
        skillManager.SkillIsInAnimation(false);

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

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefabInfo, attackIndex);
        }
    }

    public override void EndWithUnblockSkills()
    {
        BastianBaseAttackManager.OnShoot -= _onShootAction;

        _energyManager.SetCanGainEnergy(true);

        base.EndWithUnblockSkills();
    }

    void InstantiateHitBox(SkillAnimationEvent prefabInfo, int attackIndex) {
        DamageAtributes atributes = attackIndex switch {
            1 => _info.FirstAttackDamageAtributes,
            2 => _info.SecondAttackDamageAtributes,
            3 => _info.ThirdAttackDamageAtributes,
            _ => _info.FirstAttackDamageAtributes,
        };
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
            prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.ProjectileSize * Vector3.one;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.SPenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.SCritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.SLastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);
    }
}
