using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / SpinningAttack")]
public class KrakenSpinningAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenEachTentacle;
    [SerializeField] float cooldownBetweenAttacks;

    KrakenManager _krakenManager;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(SpinningAttack());

        Debug.Log("Spinning Attack");
    }

    IEnumerator SpinningAttack() {
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _krakenManager.TentaclesListGO[tentacleToHit].transform.position;
        Vector3 playerPos = _krakenManager.Player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        bool isRight = cross.y < 0;

        _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(tentacleToHit));
        yield return new WaitForSeconds(cooldownBetweenEachTentacle);

        if (!isRight) {
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count - 1; i++) {
                if (tentacleToHit == _krakenManager.ListOfTentacles.Count - 1) tentacleToHit = -1;
                tentacleToHit++;
                _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(tentacleToHit));
                yield return new WaitForSeconds(cooldownBetweenEachTentacle);
            }
        }
        else {
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count - 1; i++) {
                if (tentacleToHit == 0) tentacleToHit = _krakenManager.ListOfTentacles.Count - 1;
                tentacleToHit--;
                _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(tentacleToHit));
                yield return new WaitForSeconds(cooldownBetweenEachTentacle);
            }
        }

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
