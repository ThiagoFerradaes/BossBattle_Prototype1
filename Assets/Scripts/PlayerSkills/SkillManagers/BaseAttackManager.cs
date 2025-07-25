using UnityEngine;

public class BaseAttackManager : SkillObjectManager {

    BaseAttackSO _info;
    public override void OnStart(SkillSO skill, PlayerSkillManager skillManager) {
        _info = skill as BaseAttackSO;
    }
}
