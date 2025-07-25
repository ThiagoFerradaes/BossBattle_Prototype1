using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {
    SpearSkillSO _info;
    PlayerSkillManager _skillManager;
    Animator _anim;
    SkillSlot _slot;
    public override void OnStart(SkillSO skill, PlayerSkillManager skillManager, Animator anim, SkillSlot slot) {
        Debug.Log("Spear Test");

        Initialize(skill, skillManager, anim, slot);
        Attack();

    }

    void Initialize(SkillSO skill, PlayerSkillManager skillManager, Animator anim, SkillSlot slot) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            _skillManager = skillManager;
            _anim = anim;
            _slot = slot;
        }
    }

    void UnblockMove() {
        _skillManager.moveManager.BlockDash(false);
        _skillManager.moveManager.BlockWalk(false);
        _skillManager.moveManager.ChangeRotationType(RotationType.MoveRotation);
    }

    void Attack() {
        _anim.SetTrigger(_info.SpearAttackTriggerName);
        _skillManager.SetCooldown(_slot, _info.SpearAttackCooldown);
        UnblockMove();
    }
}
