using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / HalfAttack")]
public class KrakenHalfAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenHalfAttacks;
    [SerializeField] float cooldownBetweenAttacks;
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

        // Verificando se algum dos tentaculos está em animação
        if (_krakenManager.IsTentacleInAnimation(tentacleToHit)) {
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }
        if (_krakenManager.IsTentacleInAnimation(secondTentacleIndex)) {
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }

        // Atacando com os tentáculos
        _krakenManager.StartTentacleAttack(tentacleToHit, preparingSpeed, hitSpeed, tentacleDownTime);
        _krakenManager.StartTentacleAttack(secondTentacleIndex, preparingSpeed, hitSpeed, tentacleDownTime);

        // Esperando o tempo entre cada ataque
        yield return new WaitForSeconds(cooldownBetweenHalfAttacks);

        // Alterando os tentáculos que vão atacar
        if (cross.y >= 0) {
            tentacleToHit = (tentacleToHit - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
            secondTentacleIndex = (secondTentacleIndex + 1) % _krakenManager.TentaclesListGO.Count;
        }
        else {
            tentacleToHit = (tentacleToHit + 1) % _krakenManager.TentaclesListGO.Count;
            secondTentacleIndex = (secondTentacleIndex - 1 + _krakenManager.TentaclesListGO.Count) % _krakenManager.TentaclesListGO.Count;
        }

        // Verificando se algum dos tentaculos está em animação
        if (_krakenManager.IsTentacleInAnimation(tentacleToHit)) {
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }
        if (_krakenManager.IsTentacleInAnimation(secondTentacleIndex)) {
            yield return _krakenManager.ReturnTentacleCoroutine(tentacleToHit);
        }

        // Atacando com os tentáculos
        _krakenManager.StartTentacleAttack(tentacleToHit, preparingSpeed, hitSpeed, tentacleDownTime);
        _krakenManager.StartTentacleAttack(secondTentacleIndex, preparingSpeed, hitSpeed, tentacleDownTime);

        // Esperando a animação do ultimo tentáculo acabar
        yield return _krakenManager.ReturnTentacleCoroutine(secondTentacleIndex);

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
