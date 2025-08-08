using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Change tentacle material", 
    story: "Change [List] from [ListOfTentacles] [Material]", 
    category: "Action/Kraken", 
    id: "791d7f1a65e2e41a93daf2f3c3d1bd5c")]
public partial class ChangeTentacleMaterialAction : Action
{
    [SerializeReference] public BlackboardVariable<List<int>> List;
    [SerializeReference] public BlackboardVariable<List<GameObject>> ListOfTentacles;
    [SerializeReference] public BlackboardVariable<Material> Material;
    List<KrakenTentacle> _listOfTentacles = new();

    protected override Status OnStart()
    {
        if (List == null || ListOfTentacles == null || Material == null) {
            return Status.Failure;
        }

        _listOfTentacles.Clear();

        for (int i = 0; i < ListOfTentacles.Value.Count; i++) {
            KrakenTentacle newTentacle = new(ListOfTentacles.Value[i]);
            _listOfTentacles.Add(newTentacle);
        }

        foreach (var intValue in List.Value) {
            _listOfTentacles[intValue].SkinnedMeshRenderer.material = Material.Value;
        }

        return Status.Success;
    }
}

