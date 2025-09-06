using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianBaseAttackManager : SkillObjectManager {
    BastianBaseAttackSO _info;

    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;
    Coroutine _attackCoroutine;

    public static event Action<int> OnShoot;
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (!BastianPassiveManager.Instance.CanShoot) {
            return;
        }

        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            OnRelease(skill);
        }
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

        _attackCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        // Decidingo dano e animação baseado no attack index;
        float minDamage, maxDamage;
        string animationParameterName, animationName;
        switch (_attackIndex) {
            case 1:
                minDamage = _info.FirstAttackMinDamage;
                maxDamage = _info.FirstAttackMaxDamage;
                animationParameterName = _info.AnimationOneParameter;
                animationName = _info.AnimationOneName;
                break;
            case 2:
                minDamage = _info.SecondAttackMinDamage;
                maxDamage = _info.ThirdAttackMaxDamage;
                animationParameterName = _info.AnimationTwoParameter;
                animationName = _info.AnimationTwoName;
                break;
            case 3:
                minDamage = _info.ThirdAttackMinDamage;
                maxDamage = _info.ThirdAttackMaxDamage;
                animationParameterName = _info.AnimationThreeParameter;
                animationName = _info.AnimationThreeName;
                break;
            default:
                minDamage = _info.FirstAttackMinDamage;
                maxDamage = _info.FirstAttackMaxDamage;
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


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) {

                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

                preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

                float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
                float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
                float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ?  _info.LastOverHeatCritDamage : 0;
                float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

                DamageContext newContext = new(
                    minDamage,
                    maxDamage,
                    prefabInfo.PrefabDuration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>(),
                    new() {
                        {ExtraDamageContextAtributes.Speed, _info.ProjectileSpeed },
                        {ExtraDamageContextAtributes.Distance, _info.AttackDistance },
                        {ExtraDamageContextAtributes.Penetration, pen},
                        {ExtraDamageContextAtributes.CritChance, critChance},
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
            else {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        // Esperando a animação terminar
        while (anim.GetCurrentAnimatorStateInfo(layer).fullPathHash == attackStateHash) {
            yield return null;
        }

        // Definindo Cooldown
        float cooldown = _attackIndex < 3 ? _info.CooldownBetweenAttacks : _info.Cooldown;

        float realCooldown = cooldown / attackSpeedMultiplier;

        cooldownManager.SetCooldown(slot, realCooldown);

        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Resetando Index
        _attackIndex = _attackIndex < 3 ? _attackIndex + 1 : 1;

        // Corrotina
        _attackCoroutine = null;

        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());

        // Desbloqueando inputs
        UnblockInputs();
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }
        End();
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    void End() {
        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        gameObject.SetActive(false);
    }
}
