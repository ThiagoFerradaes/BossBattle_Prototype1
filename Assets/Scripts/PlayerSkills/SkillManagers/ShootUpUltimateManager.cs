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
            TemporaryFunction();
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

    void TemporaryFunction()
    {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        UnblockMove();

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        attackHitBox.GetComponent<ContinuosDamageHitBox>().Initialize(_info.Damage, _info.Duration, _info.EnemyTag, _info.DamageCooldown);


        gameObject.SetActive(false);
    }

    void UnblockMove()
    {
        skillManager.MoveManager.BlockDash(false);
        skillManager.MoveManager.BlockWalk(false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }

    //IEnumerator Attack()
    //{
    //    cooldownManager.SetCooldown(slot, _info.SpearAttackCooldown);
    //    anim.SetTrigger(_info.SpearAttackTriggerName);

    //    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    //    while (!stateInfo.IsName(_info.AnimationName))
    //    {
    //        yield return null;
    //        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    //    }

    //    SkillAnimationEvent prefabInfo = _info.Prefabs[0];
    //    float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;
    //    while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < targetNormalizedTime)
    //    {
    //        yield return null;
    //    }

    //    GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
    //    attackHitBox.transform.SetParent(parent.transform);
    //    attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
    //    attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(_info.Damage, _info.HitBoxDuration);

    //    while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
    //    {
    //        yield return null;
    //    }

    //    UnblockMove();
    //    gameObject.SetActive(false);
    //}
}
