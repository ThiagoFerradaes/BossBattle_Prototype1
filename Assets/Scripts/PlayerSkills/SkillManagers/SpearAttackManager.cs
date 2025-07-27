using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {
    SpearSkillSO _info;
    Animator _anim;
    SkillSlot _slot;

    public override void UseSkill(SkillSO skill, SkillSlot slot)
    {
        Debug.Log("Spear Test");

        Initialize(skill, slot);
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            StartCoroutine(Attack());
        }

    }

    void Initialize(SkillSO skill, SkillSlot slot) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            _slot = slot;
            _anim = _parent.GetComponentInChildren<Animator>();
        }
    }

    void UnblockMove() {
        _skillManager.moveManager.BlockDash(false);
        _skillManager.moveManager.BlockWalk(false);
        _skillManager.moveManager.ChangeRotationType(RotationType.MoveRotation);
    }

    IEnumerator Attack() {
        _skillManager.SetCooldown(_slot, _info.SpearAttackCooldown);
        _anim.SetTrigger(_info.SpearAttackTriggerName);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName(_info.AnimationName)) {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox; 
        while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < targetNormalizedTime) {
            yield return null;
        }

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(_parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
        attackHitBox.GetComponent<InstantDamageAttack>().Initialize(_info.Damage, _info.HitBoxDuration);

        while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockMove();
        gameObject.SetActive(false);
    }
}
