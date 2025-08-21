using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / CrossAttack")]
public class KrakenCrossAttack : EnemyBehaviourSO {
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float cooldownBetweenCrossAttacks;

    KrakenManager _krakenManager;
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        _krakenManager = parent as KrakenManager;

        _krakenManager.StartCoroutine(CrossAttack());

        Debug.Log("Cross Attack");
    }

    IEnumerator CrossAttack() {

        int tentacleToHit = _krakenManager.FindClosestTentacleToPlayer();

        bool isPair = tentacleToHit % 2 == 0;

        if (isPair) {
            for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                if (i % 2 != 0) _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(i - 1));
            }
        }
        else {
            for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                if (i % 2 == 0) _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(i - 1));
            }
        }

        yield return new WaitForSeconds(cooldownBetweenCrossAttacks);

        if (!isPair) {
            for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                if (i % 2 != 0) _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(i - 1));
            }
        }
        else {
            for (int i = 1; i < _krakenManager.ListOfTentacles.Count + 1; i++) {
                if (i % 2 == 0) _krakenManager.StartCoroutine(_krakenManager.TentacleAttack(i - 1));
            }
        }

        _krakenManager.StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);
        _krakenManager.ChangeBehaviourAtRandom();
    }
}
