using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / SpinningAttack")]
public class KrakenSpinningAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenEachTentacle;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float hitSpeed;
    [SerializeField] float preparingSpeed;
    [SerializeField] float downTime;

    KrakenManager _krakenManager;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(SpinningAttack());

    }

    IEnumerator SpinningAttack() {
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        if (_krakenManager.IsTentacleInAnimation(tentacleToHit)) { // Verificando se o tentaculo esta animando
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }

        #region Calculo de direção
        Vector3 tentaclePos = _krakenManager.TentaclesListGO[tentacleToHit].transform.position;
        Vector3 playerPos = _krakenManager.Player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        bool isRight = cross.y < 0;
        #endregion

        // Começo do ataque
        _krakenManager.StartTentacleAttack(tentacleToHit, preparingSpeed, hitSpeed, downTime);
        yield return new WaitForSeconds(cooldownBetweenEachTentacle);

        for (int i = 0; i < _krakenManager.ListOfTentacles.Count - 1; i++) {
            if (!isRight) {
                if (tentacleToHit == _krakenManager.ListOfTentacles.Count - 1) tentacleToHit = -1;
                tentacleToHit++;
            }
            else {
                if (tentacleToHit == 0) tentacleToHit = _krakenManager.ListOfTentacles.Count;
                tentacleToHit--;
            }

            _krakenManager.StartTentacleAttack(tentacleToHit, preparingSpeed, hitSpeed, downTime);

            if (i < _krakenManager.ListOfTentacles.Count - 2) {
                yield return new WaitForSeconds(cooldownBetweenEachTentacle);
            }

            else {
                yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
            }
        }

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
