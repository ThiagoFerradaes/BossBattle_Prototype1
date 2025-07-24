using UnityEngine;

public class BaseAttackManager : SkillObjectManager {

    BaseAttackSO _info;
    public override void OnStart(SkillSO skill) {
        _info = skill as BaseAttackSO;
    }
}
