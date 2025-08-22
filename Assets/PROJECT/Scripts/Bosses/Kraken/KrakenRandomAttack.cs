using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[CreateAssetMenu(menuName = "Kraken / RandomAttack")]
public class KrakenRandomAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenAttacks;
    KrakenManager _krakenManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Debug.Log("Random Attack");

        _krakenManager = parent as KrakenManager;

        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _krakenManager.TentaclesListGO[tentacleToHit].transform.position;
        Vector3 playerPos = _krakenManager.Player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        int secondTentacleIndex;

        if (cross.y < 0) {
            secondTentacleIndex = (tentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
        }
        else {
            secondTentacleIndex = (tentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
        }

        int rng = Random.Range(0, 2);

        if (rng == 1) rng = tentacleToHit;
        else rng = secondTentacleIndex;

        _krakenManager.StartCoroutine(CooldownBetweenAttacks(rng));
    }

    IEnumerator CooldownBetweenAttacks(int rng) {
        yield return _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(rng));
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
