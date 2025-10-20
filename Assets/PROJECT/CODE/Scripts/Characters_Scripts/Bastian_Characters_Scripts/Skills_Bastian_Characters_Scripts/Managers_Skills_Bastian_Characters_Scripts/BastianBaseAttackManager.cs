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

        animationCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack() {

        skillManager.SkillIsInAnimation(true);

        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        // Decidingo dano e animação baseado no attack index;
        string animationParameterName, animationName;
        DamageAtributes atributes;
        switch (_attackIndex) {
            case 1:
                atributes = _info.FirstAttackAtributes;
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
            case 2:
                atributes = _info.SecondAttackAtributes;
                animationParameterName = _info.AnimationTwoParameter;
                animationName = _info.AnimationTwoName;
                break;
            case 3:
                atributes = _info.ThirdAttackAtributes;
                animationParameterName = _info.AnimationThreeParameter;
                animationName = _info.AnimationThreeName;
                break;
            default:
                atributes = _info.FirstAttackAtributes;
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
        }

        // Animation
        anim.SetFloat(_info.AttackSpeedAnimationParameter, attackSpeedMultiplier);
        anim.SetTrigger(animationParameterName);
        AnimatorStateInfo stateInfo;

        yield return null;

        int layer = 0; // Encontrando a layer da animação
        for (int i = 0; i < anim.layerCount; i++) {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(i);

            if (state.IsName(animationName) || nextState.IsName(animationName)) {
                layer = i;
                break;
            }
        }

        do { // Esperando entrar na animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
        } while (!stateInfo.IsName(animationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Pegando a lista de prefabs e ordenando pelo tempo de spawn delas
        var prefabList = _info.Prefabs[_attackIndex];
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        // Instanciando prefabs
        for (int i = 0; i < prefabList.Count; i++) {
            SkillAnimationEvent prefabInfo = prefabList[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefabInfo, atributes);
            else InstantiateVFX(prefabInfo);
        }

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash) {
            yield return null;
        }

        FinishAttack(attackSpeedMultiplier);
    }
    void FinishAttack(float attackSpeedMultiplier) {
        // Definindo Cooldown
        float cooldown = _attackIndex < 3 ? _info.CooldownBetweenAttacks : _info.Cooldown;

        float realCooldown = cooldown / attackSpeedMultiplier;

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
    void InstantiateHitBox(SkillAnimationEvent prefabInfo, DamageAtributes atributes) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.LastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        DamageContext newContext = new(
            atributes,
            prefabInfo.PrefabDuration,
            parent.GetComponent<StatusManager>(),
            new() {
                        {ExtraDamageContextAtributes.Speed, _info.ProjectileSpeed },
                        {ExtraDamageContextAtributes.Distance, _info.AttackDistance },
                        {ExtraDamageContextAtributes.Penetration, pen},
                        {ExtraDamageContextAtributes.CritRate, critChance},
                        {ExtraDamageContextAtributes.CritDamage, critDamage}
            }
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        BastianPassiveManager.Instance.GainHeat(_info.HeatGain);

        OnShoot?.Invoke(_attackIndex);
    }
    void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
    }
}
