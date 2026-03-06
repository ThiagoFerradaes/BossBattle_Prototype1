using System;
using System.Collections;
using UnityEngine;

public class GraciaAttackManager : SkillObjectManager {
    #region Paramethers

    // Components
    GraciaAttackSO _info;

    // Ints e floats
    int _attackIndex = 1;
    float _attackSpeedMultiplier = 1;

    // Corrotinas
    Coroutine _timerBetweenAttacksCoroutine;

    // Eventos
    public static event Action OnAttackHit;

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {

        Initialize(skill);

        StartAnimation();
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaAttackSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }
    }

    void StartAnimation() {

        animationCoroutine ??= StartCoroutine(AttackCoroutine(_attackIndex - 1));
    }

    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();

        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        // DEFININDO COOLDOWN
        // Aqui o cooldown pode ser 2: 1 -> entre os ataques do combo | 2 -> cooldown do final do combo
        float cooldown = _attackIndex < 3 ? _info.CooldownBetweenAttacks : _info.Cooldown;
        float realCooldown = cooldown / _attackSpeedMultiplier; // Se a velocidade de ataque for maior então o cooldown diminui
        cooldownManager.SetCooldownSingleCharge(slot, realCooldown);

        // SETANDO O INDEX DO PROXIMO ATAQUE
        float oldIndex = _attackIndex;
        if (_attackIndex < 3) _attackIndex++; // Se for menor que 3 ele sobe em um
        else _attackIndex = 1; // Se chegou no 3 então volta pro 1

        // VOLTANDO OS INPUTS
        UnblockInputs();

        // COMEÇANDO O TEMPORIZADOR ENTRE ATAQUES
        if (oldIndex < 3) _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());
        else End();
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }

        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;

        End();
    }
    #endregion

    #region Instantiate

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        // Buscando a hitbox na pool
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        // Buscando o atributo de acordo com o ataque
        DamageAtributes atributes = _attackIndex switch {
            1 => _info.FirstAttackAtributes,
            2 => _info.SecondAttackAtributes,
            3 => _info.ThirdAttackAtributes,
            _ => _info.FirstAttackAtributes
        };

        // Settando o tamanho e a posição do ataque
        preFab.transform.localScale = atributes.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);
        preFab.transform.SetParent(null);

        // Calculando o dano do ataque
        DamageAtributes newAtribues = new(atributes);
        newAtribues.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = CalculateCritRate();
        newAtribues.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = CalculateCritDamage();
        DamageContext newContext = new(newAtribues, statusManager);


        // Ativando a hitbox
        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        // Chamando evento de uso do ataque base
        OnAttackHit?.Invoke();

        // Efeitos ao contato da hitbox
        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }

    float CalculateCritRate() {
        float attackCritRateMultiplier = _attackIndex switch {
            1 => GraciaPassiveManager.Instance.ReturnCriValues().FirstAttackCritRateValue,
            2 => GraciaPassiveManager.Instance.ReturnCriValues().SecondAttackCritRateValue,
            3 => GraciaPassiveManager.Instance.ReturnCriValues().ThirdAttackCritRateValue,
            _ => GraciaPassiveManager.Instance.ReturnCriValues().FirstAttackCritRateValue
        };
        return statusManager.ReturnStatusValue(StatusType.CritRate) + attackCritRateMultiplier;
    }

    float CalculateCritDamage() {
        float critDamage = GraciaPassiveManager.Instance.ReturnCritDamage();
        if (_attackIndex == 3) return statusManager.ReturnStatusValue(StatusType.CritDamage) + critDamage;
        else return statusManager.ReturnStatusValue(StatusType.CritDamage);
    }

    #endregion
}
