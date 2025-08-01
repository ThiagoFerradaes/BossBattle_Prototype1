using System;
using System.Collections;
using UnityEngine;

public class ShootUpUltimateManager : SkillObjectManager
{
    #region Parameter

    // Components
    ShootUpUltimateSO _info;
    WeaponManager _weaponManager;

    // Coroutines
    Coroutine _attackCoroutine;

    // Events
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

    public override void SetSkillRangeIndicator(SkillSO skill)
    {
        currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        currentSkillRange.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as ShootUpUltimateSO;

        if(_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>();
    }


    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        anim.SetTrigger(_info.AnimationParameterTrigger);

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition);
        _weaponManager.OnEquipLeftHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        ContinuosDamageContext newContext = new(
            ReturnSkillDamage(_info.Damage),
            _info.Duration,
            _info.DamageCooldown,
            false,
            _info.EnemyTag
            );

        attackHitBox.GetComponent<ContinuosDamageHitBox>().Initialize(newContext);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.LastAnimationName));

        attackStateHash = stateInfo.fullPathHash;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipLeftHand();
        _weaponManager.OnDesequipRightHand();

        UnblockInputs();
        _attackCoroutine = null;
        OnWeaponChange?.Invoke();
        gameObject.SetActive(false);
    }

    #endregion
}
