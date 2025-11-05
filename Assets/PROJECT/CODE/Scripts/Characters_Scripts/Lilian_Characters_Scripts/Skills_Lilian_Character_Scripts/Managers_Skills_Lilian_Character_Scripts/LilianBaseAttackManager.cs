using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LilianBaseAttackManager : SkillObjectManager
{
    // Components
    LilianBaseAttackSO _info;
    HealthManager _healthManager;

    // Atributes
    int _attackIndex = 1;
    float _attackSpeedMultiplier;

    #region Initialize
    public override void UseSkill(SkillSO skill)
    {
        base.UseSkill(skill);

        Initialize(skill);

        string animationParameterName, animationName;
        switch (_attackIndex)
        {
            case 1:
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
            case 2:
                animationParameterName = _info.AnimationTwoParameter;
                animationName = _info.AnimationTwoName;
                break;
            default:
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
        }

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, animationParameterName, animationName, 0));
    }

    private void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as LilianBaseAttackSO;
        if (_healthManager == null) _healthManager = parent.GetComponent<HealthManager>();

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }


    }

    #endregion

    #region Skill

    public override void FirstFunc()
    {
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
        skillManager.SkillIsInAnimation(true);
    }

    public override void ThirdFunc()
    {
        float healthToLoose = _healthManager.ReturnCurrentHealth() * _info.DamagePercentToDamageLilian / 100;
        _healthManager.TakeDamage(healthToLoose);
    }

    public override void FourthFunc()
    {
        // Cooldown
        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);

        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Attack Index
        switch (_attackIndex)
        {
            case 1:
                _attackIndex = 2;
                break;
            case 2:
                _attackIndex = 1;
                break;
        }

        // Corrotina
        animationCoroutine = null;

        // Avisando que não está mais em animação
        skillManager.SkillIsInAnimation(false);

        // Desbloqueando inputs
        UnblockInputs();
    }

    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo)
    {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.SkillDamageAtributes.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);


        DamageContext newContext = new(
            _info.SkillDamageAtributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }
    #endregion
}
