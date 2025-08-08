using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeTentacleList",
    story: "Change [List] based on [Index] , [Attack] , [First] , [Rotation] and [ListOfTentacles]",
    category: "Action/Kraken",
    id: "5104f4f244c72892620db76f303a9d3c")]
public partial class ChangeTentacleListAction : Action {
    [SerializeReference] public BlackboardVariable<List<int>> List;
    [SerializeReference] public BlackboardVariable<int> Index;
    [SerializeReference] public BlackboardVariable<KrakenAttack> Attack;
    [SerializeReference] public BlackboardVariable<bool> First;
    [SerializeReference] public BlackboardVariable<TypeOfRotation> Rotation;
    [SerializeReference] public BlackboardVariable<List<GameObject>> ListOfTentacles;

    protected override Status OnStart() {

        if (List == null || Index == null || Attack == null || Rotation == null) {
            return Status.Failure;
        }

        List.Value.Clear();
        int currentIndex = Index.Value;

        switch (Attack.Value) {
            case KrakenAttack.HalfArena when First.Value:
                int nextTentacle = Rotation.Value == TypeOfRotation.Clock ? currentIndex + 1 : currentIndex - 1; 
                List.Value.Add(currentIndex);
                List.Value.Add(nextTentacle);
                break;
            case KrakenAttack.HalfArena when !First.Value:
                currentIndex = Rotation.Value == TypeOfRotation.Clock ? currentIndex - 1 : currentIndex + 1;
                nextTentacle = Rotation.Value == TypeOfRotation.Clock ? currentIndex + 3 : currentIndex - 3;
                List.Value.Add(currentIndex);
                List.Value.Add(nextTentacle);
                break;
            case KrakenAttack.Cross when First.Value:
                bool isPair = currentIndex % 2 == 0;
                for (int i = 0; i < ListOfTentacles.Value.Count; i++) {
                    if (isPair && i % 2 == 0) List.Value.Add(i);
                    else if (!isPair && i % 2 != 0) List.Value.Add(i);
                }
                break;
            case KrakenAttack.Cross when !First.Value:
                isPair = currentIndex % 2 == 0;
                for (int i = 0; i < ListOfTentacles.Value.Count; i++) {
                    if (!isPair && i % 2 == 0) List.Value.Add(i);
                    else if (isPair && i % 2 != 0) List.Value.Add(i);
                }
                break;
        }


        return Status.Success;
    }
}

