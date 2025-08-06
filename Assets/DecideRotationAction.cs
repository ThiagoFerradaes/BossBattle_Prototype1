using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Decide Rotation", story: "Decide [Rotation] based on [List] [Index]", category: "Action/Kraken", id: "e76f3ec5ddf265fe2ac83f21841c150c")]
public partial class DecideRotationAction : Action {
    [SerializeReference] public BlackboardVariable<TypeOfRotation> Rotation;
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;
    [SerializeReference] public BlackboardVariable<int> Index;
    Transform _player;

    protected override Status OnStart() {

        if (Rotation == null || List == null || Index == null) {
            return Status.Failure;
        }

        _player = PlayerSpawnManager.Instance.Player.transform;

        Vector3 tentaclePos = List.Value[Index.Value].transform.position;
        Vector3 playerPos = _player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        Rotation.Value = cross.y > 0 ? TypeOfRotation.Clock : TypeOfRotation.CounterClock;

        return Status.Success;
    }

}

