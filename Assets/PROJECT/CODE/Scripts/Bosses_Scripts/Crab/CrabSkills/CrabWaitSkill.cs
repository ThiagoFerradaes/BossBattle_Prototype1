using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaitSkill")]
public class CrabWaitSkill : EnemyBehaviourSO {

    CrabManager _crabManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
    }

}
