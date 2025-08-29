using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / CrossAttack")]
public class KrakenCrossAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float cooldownBetweenCrossAttacks;
    [SerializeField] float preparingSpeed;
    [SerializeField] float hitSpeed;
    [SerializeField] float tentacleDownTime;
    [SerializeField] float repetitionAmount;
    [SerializeField] float cooldownBetweenRepetitions;

    KrakenManager _krakenManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(CrossAttack());

    }

    IEnumerator CrossAttack() {        

        for (int j = 0; j < repetitionAmount; j++) {
            int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

            bool isPair = tentacleToHit % 2 == 0;

            if (isPair) {
                for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                    if (i % 2 != 0) _krakenManager.StartTentacleAttack((i - 1), preparingSpeed, hitSpeed, tentacleDownTime);
                }
            }
            else {
                for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                    if (i % 2 == 0) _krakenManager.StartTentacleAttack((i - 1), preparingSpeed, hitSpeed, tentacleDownTime);
                }
            }

            yield return new WaitForSeconds(cooldownBetweenCrossAttacks);

            if (!isPair) {
                for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                    if (i % 2 != 0) _krakenManager.StartTentacleAttack((i - 1), preparingSpeed, hitSpeed, tentacleDownTime);
                }

                yield return _krakenManager.ReturnTentacleCoroutine(1);
            }
            else {
                for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                    if (i % 2 == 0) _krakenManager.StartTentacleAttack((i - 1), preparingSpeed, hitSpeed, tentacleDownTime);
                }

                yield return _krakenManager.ReturnTentacleCoroutine(0);
            }

            if (j < repetitionAmount - 1) {
                yield return new WaitForSeconds(cooldownBetweenRepetitions);
            }
        }

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
