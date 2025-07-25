using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {
    SpearSkillSO _info;
    PlayerSkillManager _skillManager;
    Animator _anim;
    SkillSlot _slot;
    Transform _parent;
    public override void OnStart(SkillSO skill, PlayerSkillManager skillManager, Animator anim, SkillSlot slot) {
        Debug.Log("Spear Test");

        Initialize(skill, skillManager, anim, slot);
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
            StartCoroutine(Attack());
        }

    }

    void Initialize(SkillSO skill, PlayerSkillManager skillManager, Animator anim, SkillSlot slot) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            _skillManager = skillManager;
            _anim = anim;
            _slot = slot;
            _parent = _skillManager.transform;
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
        attackHitBox.transform.SetParent(_parent);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
        attackHitBox.GetComponent<InstantDamageAttack>().Initialize(_info.Damage, _info.HitBoxDuration);

        while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockMove();
        gameObject.SetActive(false);
    }
}
