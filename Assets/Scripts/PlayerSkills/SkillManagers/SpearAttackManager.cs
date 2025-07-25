using UnityEngine;

public class SpearAttackManager : SkillObjectManager
{
    public override void OnStart(SkillSO skill, PlayerSkillManager skillManager) {
        Debug.Log("Spear Test");
        skillManager.moveManager.BlockDash(false);
        skillManager.moveManager.BlockWalk(false);
        skillManager.moveManager.ChangeRotationType(RotationType.MoveRotation);
    }

}
