using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianBaseAttackManager : SkillObjectManager {

    // Components
    BastianBaseAttackSO _info;

    // Atributes
    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;

    // Actions
    public static event Action<int> OnShoot;

    float _attackSpeedMultiplier;
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (!BastianPassiveManager.Instance.CanShoot) {
            return;
        }

        base.HandleInput(skill, ctx);
    }
    public override void UseSkill(SkillSO skill) {


        if (_info == null) _info = skill as BastianBaseAttackSO;

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }

        string animationParameterName, animationName;
        switch (_attackIndex) {
            case 1:
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
            case 2:
                animationParameterName = _info.AnimationTwoParameter;
                animationName = _info.AnimationTwoName;
                break;
            case 3:
                animationParameterName = _info.AnimationThreeParameter;
                animationName = _info.AnimationThreeName;
                break;
            default:
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
        }
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, animationParameterName, animationName, _attackIndex));
    }

    public override void FirstFunc() {
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
        skillManager.SkillIsInAnimation(true);
    }
    public override void FourthFunc() {

        // Definindo Cooldown
        float cooldown = _attackIndex < 3 ? _info.CooldownBetweenAttacks : _info.Cooldown;

        float realCooldown = cooldown / _attackSpeedMultiplier;

        cooldownManager.SetCooldownSingleCharge(slot, realCooldown);

        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Resetando Index
        _attackIndex = _attackIndex < 3 ? _attackIndex + 1 : 1;

        // Corrotina
        animationCoroutine = null;

        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());

        // Desbloqueando inputs
        UnblockInputs();

        // Avisando que não está mais em animação
        skillManager.SkillIsInAnimation(false);
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }
        EndWithUnblockSkills();
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    public override void CancelSkill() {

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }

        _attackIndex = 1;
        base.CancelSkill();
    }
    public override void EndWithUnblockSkills() {
        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        base.EndWithUnblockSkills();
    }
    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = Vector3.one * _info.ProjectileSize;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.LastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        DamageAtributes atributes = _attackIndex switch
        {
            1 => _info.FirstAttackAtributes,
            2 => _info.SecondAttackAtributes,
            3 => _info.ThirdAttackAtributes,
            _ => _info.FirstAttackAtributes,
        };
        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = critChance;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        BastianPassiveManager.Instance.GainHeat(_info.HeatGain);

        OnShoot?.Invoke(_attackIndex);
    }
    public override void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
    }
}
