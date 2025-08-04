using System;
using System.Collections;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    SpearSkillSO _info;
    WeaponManager _weaponManager;

    // Coroutines
    Coroutine _attackCoroutine;

    // Event
    public static event Action OnWeaponChange;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill)
    {

        Initialize(skill);
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            _attackCoroutine ??= StartCoroutine(Attack());
        }

    }

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }
    }

    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        anim.SetTrigger(_info.SpearAttackTriggerName);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        _weaponManager.OnEquipRightHand(_info.SpearPrefab, _info.SpearName, _info.WeaponPosition);

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);

        InstantDamageContext newContext = new(
            ReturnSkillDamage(_info.Damage),
            _info.HitBoxDuration,
            _info.IsTrueDamage,
            _info.EnemyTag
            );

        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

        OnWeaponChange?.Invoke();

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipRightHand();
        UnblockInputs();
        _attackCoroutine = null;
        gameObject.SetActive(false);
    }


    #endregion
}
