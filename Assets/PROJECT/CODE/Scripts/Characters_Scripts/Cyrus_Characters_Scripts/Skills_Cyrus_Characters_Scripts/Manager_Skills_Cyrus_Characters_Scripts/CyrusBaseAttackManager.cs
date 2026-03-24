using System.Collections;
using UnityEngine;

public class CyrusBaseAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusBaseAttackSO _info;

    // Int
    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;

    float _attackSpeedMultiplier;
    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {

        Initialize(skill);

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }
        animationCoroutine ??= StartCoroutine(AttackCoroutine(_attackIndex - 1, _attackIndex));
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusBaseAttackSO;
        }

    }

    protected override void FirstFunc() {
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();

        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);

        base.FirstFunc();
    }


    protected override void FourthFunc() {
        float cooldown = _attackIndex == 1 ? _info.CooldownBetweenAttacks : _info.Cooldown;
        float realCooldown = cooldown / _attackSpeedMultiplier;

        cooldownManager.SetCooldownSingleCharge(slot, realCooldown);

        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        _attackIndex = _attackIndex == 1 ? _attackIndex = 2 : _attackIndex = 1;

        UnblockInputs();

        animationCoroutine = null;

        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());

        skillManager.SkillIsInAnimation(false);
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

    public override void CancelSkill() {
        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;

        base.CancelSkill();
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        DamageAtributes atributes = _attackIndex == 1 ? _info.FirstAttackAtributes : _info.SecondAttackAtributes;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }
    #endregion
}
