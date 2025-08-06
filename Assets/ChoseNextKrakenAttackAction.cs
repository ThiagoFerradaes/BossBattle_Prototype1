using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChoseNextKrakenAttack", story: "Set [NextAttack] based on [ListOfAttacks]", category: "Action/Blackboard/Kraken", id: "7fb385c4ea28c4cb7e76b4dfb9c6f420")]
public partial class ChoseNextKrakenAttackAction : Action
{
    [SerializeReference] public BlackboardVariable NextAttack;
    [SerializeReference] public BlackboardVariable ListOfAttacks;
    ListOfEnemyAttacksSO listOfAttacks;
    protected override Status OnStart()
    {
        if (ListOfAttacks == null || NextAttack == null) {
            return Status.Failure;
        }

        if (ListOfAttacks is not BlackboardVariable<ListOfEnemyAttacksSO>) {
            return Status.Failure;
        }

        listOfAttacks = ListOfAttacks as BlackboardVariable<ListOfEnemyAttacksSO>;
        List<EnemySkillSO> list = listOfAttacks.ListOfEnemyAttacks;
        var sortedSkills = list.OrderByDescending(skill => skill.Priority);
        var cooldownManager = EnemyCooldownManager.Instance;

        foreach (var skill in sortedSkills) {
            if (!cooldownManager.SkillInCooldown(skill)) {
                if (skill is KrakenSkillSO krakenSkill) {
                    NextAttack.ObjectValue = krakenSkill.KrakenAttack;
                }

                cooldownManager.SetSkillCooldown(skill);
                return Status.Success;
            }
        }

        return Status.Failure;
    }

}

