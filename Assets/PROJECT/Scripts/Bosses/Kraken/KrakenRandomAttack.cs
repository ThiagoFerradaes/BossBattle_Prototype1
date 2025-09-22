using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / RandomAttack")]
public class KrakenRandomAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float preparingSpeed;
    [SerializeField] float hitSpeed;
    [SerializeField] float tentacleDownTime;
    KrakenManager _krakenManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        if(_krakenManager == null) _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(RandomAttack());
    }

    IEnumerator RandomAttack() {
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        if (_krakenManager.IsTentacleInAnimation(tentacleToHit)) {
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }

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

        _krakenManager.StartTentacleAttack(rng, preparingSpeed, hitSpeed, tentacleDownTime);

        yield return _krakenManager.ReturnTentacleCoroutine(rng);

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
