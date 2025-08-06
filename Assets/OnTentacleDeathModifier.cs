using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OnTentacleDeath", story: "Execute child if one of [Tentacle] dies", category: "Flow/Kraken", id: "58d08edb9d41665ab5f580736e46c3a7")]
public partial class OnTentacleDeathModifier : Modifier {
    [SerializeReference] public BlackboardVariable<List<GameObject>> Tentacle;
    bool _eventTriggered;
    List<KrakenTentacle> _listOfTentacles = new();

    protected override Status OnStart() {

        _eventTriggered = false;

        if (Tentacle == null || Child == null) {
            return Status.Failure;
        }

        for (int i = 0; i < Tentacle.Value.Count; i++) {
            KrakenTentacle newTentacle = new(Tentacle.Value[i]);
            _listOfTentacles.Add(newTentacle);
        }
        
        foreach (var tentacle in _listOfTentacles) {
            HealthManager health = tentacle.Health;
            health.OnDeath += HandleTentacleDeath;
        }
        return Status.Running;
    }

    private void HandleTentacleDeath() {
        _eventTriggered = true;
    }

    protected override Status OnUpdate() {
        if (_eventTriggered) {
            var childStatus = StartNode(Child);

            if (childStatus == Status.Success || childStatus == Status.Failure) {
                _eventTriggered = false;
                return Status.Running;
            }

            return childStatus;

        }

        return Status.Running;
    }

    protected override void OnEnd() {
        foreach (var t in _listOfTentacles) {
            HealthManager health = t.Health;
            health.OnDeath -= HandleTentacleDeath;
        }
    }
}

