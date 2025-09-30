using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ BigClaw")]
public class CrabBigClawAttack : EnemyBehaviourSO {

    CrabManager _crabManager;
    Animator _anim;

    [SerializeField] float distanceToPlayer = 2;
    [SerializeField] float cooldownBetweenThisAttackAndNext = 2;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        if (CrabArenaManager.Instance.ReturnCurrentTide() != CrabArenaState.LowTide) {
            _crabManager.CooldownManager.SetSkillCooldown(this);
            _crabManager.ChangeBehaviourAtRandom();
        }
        else {
            _crabManager.StartCoroutine(WalkToPlayer());
        }

    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;

    }

    IEnumerator WalkToPlayer() {

        _crabManager.WalkToPlayer(distanceToPlayer);

        yield return _crabManager.ReturnWalkCoroutine();

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenThisAttackAndNext);
        _crabManager.ChangeBehaviourAtRandom();

    }
}
