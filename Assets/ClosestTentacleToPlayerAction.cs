using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Closest Tentacle To Player",
    description: "Chooses the closest tentacle from a list to the player, and then changes the index",
    story: "Closest [list] to player, changes [index]", 
    category: "Action/Kraken", 
    id: "2bbf5116f91d6873e16573978c1a1760")]
public partial class ClosestTentacleToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;
    [SerializeReference] public BlackboardVariable<int> Index;
    Transform _player;

    protected override Status OnStart()
    {
        _player = PlayerSpawnManager.Instance.Player.transform;

        int tentacleIndex = -1;
        float distance = Mathf.Infinity;

        for (int i = 0; i < List.Value.Count; i++) {
            if (List.Value[i] == null) continue;

            float newDistance = Vector3.Distance(List.Value[i].transform.position, _player.position);
            if (newDistance < distance) {
                distance = newDistance;
                tentacleIndex = i;
            }
        }

        Index.Value = tentacleIndex;

        return Status.Success;
    }

}

