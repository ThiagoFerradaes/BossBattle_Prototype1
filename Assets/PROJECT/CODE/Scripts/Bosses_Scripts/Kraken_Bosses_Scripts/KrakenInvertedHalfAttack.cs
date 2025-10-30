using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ Behaviour/ Kraken / InvertedHalfAttack")]
public class KrakenInvertedHalfAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenHalfAttacks;
    [SerializeField] float preparingSpeed;
    [SerializeField] float hitSpeed;
    [SerializeField] float tentacleDownTime;

    KrakenManager _krakenManager;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(HalfAttack());
    }

    IEnumerator HalfAttack() {
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();
        int realTentacleToHit = (tentacleToHit + 4) % _krakenManager.TentaclesListGO.Count;

        if (_krakenManager.IsTentacleInAnimation(realTentacleToHit)) {
            yield return _krakenManager.ReturnTentacleCoroutine(realTentacleToHit);
        }

        Vector3 tentaclePos = _krakenManager.TentaclesListGO[realTentacleToHit].transform.position;
        Vector3 playerPos = _krakenManager.Player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        int secondTentacleIndex;

        if (cross.y > 0) {
            secondTentacleIndex = (realTentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
        }
        else {
            secondTentacleIndex = (realTentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
        }

        _krakenManager.StartTentacleAttack(realTentacleToHit, preparingSpeed, hitSpeed, tentacleDownTime);
        _krakenManager.StartTentacleAttack(secondTentacleIndex, preparingSpeed, hitSpeed, tentacleDownTime);

        yield return new WaitForSeconds(cooldownBetweenHalfAttacks);

        for (int j = 0; j < 3; j++) {
            if (cross.y <= 0) {
                realTentacleToHit = (realTentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
                secondTentacleIndex = (secondTentacleIndex + 1) % _krakenManager.TentaclesListGO.Count;
            }
            else {
                realTentacleToHit = (realTentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
                secondTentacleIndex = (secondTentacleIndex - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
            }

            _krakenManager.StartTentacleAttack(realTentacleToHit, preparingSpeed, hitSpeed, tentacleDownTime);
            _krakenManager.StartTentacleAttack(secondTentacleIndex, preparingSpeed, hitSpeed, tentacleDownTime);

            yield return new WaitForSeconds(cooldownBetweenHalfAttacks);
        }
        yield return _krakenManager.ReturnTentacleCoroutine(secondTentacleIndex);

        _krakenManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }

}
