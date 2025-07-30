using System.Collections;
using UnityEngine;

public class WeaponMasterBaseAttack : SkillObjectManager {

    #region Parameters

    // Components
    WeaponMasterBaseAttackSO _info;

    // Int
    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;
    Coroutine _attackCoroutine;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        if (_info == null) _info = skill as WeaponMasterBaseAttackSO;

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

        Debug.Log(attackSpeedMultiplier);

        float cooldown = _info.CooldownBetweenAttacks / attackSpeedMultiplier;
        cooldownManager.SetCooldown(slot, cooldown);

        string animationParameter = _attackIndex == 1 ? _info.FirstBaseAttackParameter : _info.SecondBaseAttackParameter;
        string animationName = _attackIndex == 1 ? _info.FirstBaseAttackAnimationName : _info.SecondtBaseAttackAnimationName;

        anim.speed = attackSpeedMultiplier;

        anim.SetTrigger(animationParameter);

        AnimatorStateInfo stateInfo;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(animationName));

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _attackIndex == 1 ? _info.Prefabs[0] : _info.Prefabs[1];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

        Vector3 hitBoxPosition = _attackIndex == 1 ? _info.FirstBaseAttackHitBoxPosition : _info.SecondtBaseAttackHitBoxPosition;
        float attackDamage = _attackIndex == 1 ? _info.FirstAttackDamage : _info.SecondAttackDamage;

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(hitBoxPosition, Quaternion.identity);

        InstantDamageContext newContext = new(
            ReturnSkillDamage(attackDamage),
            0.1f,
            _info.IsTrueDamage,
            _info.EnemyTag
            );

        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        anim.speed = 1f;

        _attackIndex = _attackIndex == 1 ? _attackIndex = 2 : _attackIndex = 1;

        UnblockInputs();

        _attackCoroutine = null;
        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }

        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        gameObject.SetActive(false);
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed)/100;
        return Mathf.Max(0.1f, baseSpeed);
    }
    #endregion
}
