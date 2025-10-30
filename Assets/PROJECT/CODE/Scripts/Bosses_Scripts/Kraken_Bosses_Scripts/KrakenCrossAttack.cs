using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ Behaviour/ Kraken / CrossAttack")]
public class KrakenCrossAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenCrossAttacks;
    [SerializeField] float preparingSpeed;
    [SerializeField] float hitSpeed;
    [SerializeField] float tentacleDownTime;
    [SerializeField] float repetitionAmount;
    [SerializeField] float cooldownBetweenRepetitions;

    KrakenManager _krakenManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        if (_krakenManager == null) _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(CrossAttack());

    }

    IEnumerator CrossAttack() {
        // Pegando o tentáculo mais próximo do jogador
        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        // Verificando se o index dele é par ou impar
        bool isPair = tentacleToHit % 2 == 0;

        for (int j = 0; j < repetitionAmount; j++) {

            // Verificando se algum dos tentáculos que será utilizado está em animação
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count; i++) {
                if (isPair && i % 2 == 0 || !isPair && i % 2 != 0) {
                    if (_krakenManager.IsTentacleInAnimation(i)) yield return _krakenManager.ReturnTentacleCoroutine(i); 
                }
            }

            // Batendo com os tentáculos 
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count; i++) {
                if (isPair && i % 2 == 0 || !isPair && i % 2 != 0) {
                    _krakenManager.StartTentacleAttack((i), preparingSpeed, hitSpeed, tentacleDownTime);
                }
            }

            // Espera o tempo entre os ataques
            yield return new WaitForSeconds(cooldownBetweenCrossAttacks);

            // Verificando se algum dos tentáculos que será utilizado está em animação
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count; i++) {
                if (!isPair && i % 2 == 0 || isPair && i % 2 != 0) {
                    if (_krakenManager.IsTentacleInAnimation(i)) yield return _krakenManager.ReturnTentacleCoroutine(i);
                }
            }

            // Batendo com os tentáculos opostos ao primeiro ataque
            for (int i = 0; i < _krakenManager.ListOfTentacles.Count; i++) {
                if (!isPair && i % 2 == 0 || isPair && i % 2 != 0) {
                    _krakenManager.StartTentacleAttack((i), preparingSpeed, hitSpeed, tentacleDownTime);
                }
            }

            if (j < repetitionAmount - 1) {
                yield return new WaitForSeconds(cooldownBetweenRepetitions);
            }
        }

        // Verificando se algum dos tentáculos que será utilizado está em animação
        for (int i = 0; i < _krakenManager.ListOfTentacles.Count; i++) {
            if (!isPair && i % 2 == 0 || isPair && i % 2 != 0) {
                if (_krakenManager.IsTentacleInAnimation(i)) yield return _krakenManager.ReturnTentacleCoroutine(i);
            }
        }

        _krakenManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }

}
