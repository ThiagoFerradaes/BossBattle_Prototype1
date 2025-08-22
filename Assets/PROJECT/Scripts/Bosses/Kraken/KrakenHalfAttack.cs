using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / HalfAttack")]
public class KrakenHalfAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenHealfAttacks;
    [SerializeField] float cooldownBetweenAttacks;

    KrakenManager _krakenManager;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(HalfAttack());

        Debug.Log("Half Attack");
    }

    IEnumerator HalfAttack() {
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _krakenManager.TentaclesListGO[tentacleToHit].transform.position;
        Vector3 playerPos = _krakenManager.Player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        int secondTentacleIndex;
        bool isRight = false;

        if (cross.y < 0) {
            secondTentacleIndex = (tentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
            isRight = false;
        }
        else {
            secondTentacleIndex = (tentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
            isRight = true;
        }

        _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(tentacleToHit));
        _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(secondTentacleIndex));

        yield return new WaitForSeconds(cooldownBetweenHealfAttacks);

        if (isRight) {
            tentacleToHit = (tentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
            secondTentacleIndex = (secondTentacleIndex + 1) % _krakenManager.TentaclesListGO.Count;
        }
        else {
            tentacleToHit = (tentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
            secondTentacleIndex = (secondTentacleIndex - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
        }

        _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(tentacleToHit));
        yield return _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(secondTentacleIndex));

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
