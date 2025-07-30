using System;
using System.Collections;
using UnityEngine;

public class ShootUpUltimateManager : SkillObjectManager
{
    #region Parameter

    // Components
    ShootUpUltimateSO _info;

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
        if (_info == null)
        {
            _info = skill as ShootUpUltimateSO;
        }
    }


    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        anim.SetTrigger(_info.AnimationParameterTrigger);

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

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockInputs();
        _attackCoroutine = null;
        OnWeaponChange?.Invoke();
        gameObject.SetActive(false);
    }

    #endregion
}
