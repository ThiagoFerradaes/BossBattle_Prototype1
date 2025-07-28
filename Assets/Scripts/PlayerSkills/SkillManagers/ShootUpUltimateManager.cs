using System.Collections;
using UnityEngine;

public class ShootUpUltimateManager : SkillObjectManager
{
    ShootUpUltimateSO _info;

    public override void UseSkill(SkillSO skill)
    {
        Debug.Log("Ultimate Test");

        Initialize(skill);
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            StartCoroutine(Attack());
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
        while (!stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < targetNormalizedTime) {
            yield return null;
        }

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        attackHitBox.GetComponent<ContinuosDamageHitBox>().Initialize(_info.Damage, _info.Duration, _info.EnemyTag, _info.DamageCooldown);

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockInputs();
        gameObject.SetActive(false);
    }
}
