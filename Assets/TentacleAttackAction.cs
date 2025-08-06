using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TentacleAttack", 
    story: "Perform an [Attack] from index: [Tentacle] " +
    "from list: [ListOfTentacles] using [KrakenObject]", 
    category: "Action/Kraken", 
    id: "106c35dc0f73da6ae018dd478b4b37c9")]
public partial class TentacleAttackAction : Action {
    [SerializeReference] public BlackboardVariable<int> Tentacle;
    [SerializeReference] public BlackboardVariable<KrakenTentacleAttack> Attack;
    [SerializeReference] public BlackboardVariable<GameObject> KrakenObject;
    [SerializeReference] public BlackboardVariable<List<GameObject>> ListOfTentacles;

    List<KrakenTentacle> _listOfTentacles = new();

    private Coroutine _coroutine;

    protected override Status OnStart() {
        var owner = KrakenObject.Value.GetComponent<BehaviorGraphAgent>();

        if (ListOfTentacles != null) {
            for (int i = 0; i < ListOfTentacles.Value.Count; i++) {
                KrakenTentacle newTentacle = new(ListOfTentacles.Value[i]);
                _listOfTentacles.Add(newTentacle);
            }
        }

        if (owner != null) {
            _coroutine = owner.StartCoroutine(TentacleAttack());
            return Status.Success;
        }
        else {
            Debug.LogError("TentacleAttackAction: Context is not a MonoBehaviour.");
        }

        return Status.Failure;
    }

    IEnumerator TentacleAttack() {

        int currentTentacleIndex = Tentacle.Value;
        Animator anim = ListOfTentacles.Value[Tentacle.Value].GetComponentInChildren<Animator>();
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
        float yRotation = 180 + (currentTentacleIndex * 45);
        attackHitBox.transform.SetPositionAndRotation(new Vector3(0, 3, 0), Quaternion.Euler(90, yRotation, 0));

        InstantDamageContext newContext = new(
            Attack.Value.TentacleDamage,
            0.1f,
            false,
            Tags.Player
            );

        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

        _listOfTentacles[currentTentacleIndex].HitBox.SetActive(true);

        yield return new WaitForSeconds(3);

        anim.SetTrigger(Attack.Value.ReturnToIdleAnimationParameter);

        _listOfTentacles[currentTentacleIndex].HitBox.SetActive(false);
    }
}

