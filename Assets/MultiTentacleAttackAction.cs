using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Multi Tentacle Attack", story: "[TentacleList] [Attack] using [ListOfTentacles] and [Self]", category: "Action/Kraken", id: "68a06ec608eab0f964e15ff35d277275")]
public partial class MultiTentacleAttackAction : Action {
    [SerializeReference] public BlackboardVariable<List<int>> TentacleList;
    [SerializeReference] public BlackboardVariable<KrakenTentacleAttack> Attack;
    [SerializeReference] public BlackboardVariable<List<GameObject>> ListOfTentacles;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    List<KrakenTentacle> _listOfTentacles = new();

    protected override Status OnStart() {
        var owner = Self.Value.GetComponent<BehaviorGraphAgent>();

        if (ListOfTentacles != null) {
            for (int i = 0; i < ListOfTentacles.Value.Count; i++) {
                KrakenTentacle newTentacle = new(ListOfTentacles.Value[i]);
                _listOfTentacles.Add(newTentacle);
            }
        }

        if (owner != null) {
            foreach (var t in TentacleList.Value) {
                owner.StartCoroutine(TentacleAttack(t));
            }
            return Status.Success;
        }
        else {
            Debug.LogError("TentacleAttackAction: Context is not a MonoBehaviour.");
        }

        return Status.Failure;
    }

    IEnumerator TentacleAttack(int tentacleIndex) {

        Animator anim = ListOfTentacles.Value[tentacleIndex].GetComponentInChildren<Animator>();
        anim.SetTrigger(Attack.Value.AttackAnimationParameter);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(Attack.Value.AttackAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(Attack.Value.AttackHitAnimationName));

        attackStateHash = stateInfo.fullPathHash;
        SkillAnimationEvent skillEvent = Attack.Value._listOfPrefabs[0];

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < skillEvent.timeToSpawnHitBox);

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(skillEvent.hitboxName, skillEvent.hitboxPrefab);
        float yRotation = 180 + (tentacleIndex * 45);
        attackHitBox.transform.SetPositionAndRotation(new Vector3(0, 3, 0), Quaternion.Euler(90, yRotation, 0));

        InstantDamageContext newContext = new(
            Attack.Value.TentacleDamage,
            0.1f,
            false,
            Tags.Player
            );

        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

        _listOfTentacles[tentacleIndex].HitBox.SetActive(true);

        yield return new WaitForSeconds(3);

        anim.SetTrigger(Attack.Value.ReturnToIdleAnimationParameter);

        _listOfTentacles[tentacleIndex].HitBox.SetActive(false);
    }
}

