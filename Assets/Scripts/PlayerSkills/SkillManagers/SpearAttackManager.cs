using System.Collections;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {
    SpearSkillSO _info;

    public override void UseSkill(SkillSO skill)
    {
        Debug.Log("Spear Test");

        Initialize(skill);
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            StartCoroutine(Attack());
        }

    }

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
        }
    }

    void UnblockMove() {
        skillManager.MoveManager.BlockDash(false);
        skillManager.MoveManager.BlockWalk(false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }

    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.SpearAttackCooldown);
        anim.SetTrigger(_info.SpearAttackTriggerName);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox; 
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < targetNormalizedTime) {
            yield return null;
        }

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(_info.Damage, _info.HitBoxDuration);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockMove();
        gameObject.SetActive(false);
    }
}
