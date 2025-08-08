using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OnTentacleDeath",
    story: "Execute child if one of [Tentacle] dies and add it to [ListOfDead]",
    category: "Flow/Kraken",
    id: "58d08edb9d41665ab5f580736e46c3a7")]
public partial class OnTentacleDeathModifier : Modifier {
    [SerializeReference] public BlackboardVariable<List<GameObject>> Tentacle;
    [SerializeReference] public BlackboardVariable<List<int>> ListOfDead;

    bool _childRunning;
    Dictionary<int, KrakenTentacle> _listOfTentacles = new();
    Dictionary<int, System.Action> _deathHandlers = new();
    int _numberOfDeadTentacles;

    protected override Status OnStart() {
        _childRunning = false;
        _numberOfDeadTentacles = 0;

        if (Tentacle == null || Child == null || ListOfDead == null)
            return Status.Failure;

        for (int i = 0; i < Tentacle.Value.Count; i++) {
            KrakenTentacle newTentacle = new(Tentacle.Value[i]);
            if (!_listOfTentacles.ContainsKey(i))
                _listOfTentacles[i] = newTentacle;
        }

        foreach (var tentacle in _listOfTentacles) {
            int index = tentacle.Key;
            HealthManager health = tentacle.Value.Health;

            if (!_deathHandlers.ContainsKey(index)) {
                System.Action handler = () => HandleTentacleDeath(index);
                _deathHandlers[index] = handler;
                health.OnDeath += handler;
            }
        }

        return Status.Running;
    }

    private void HandleTentacleDeath(int tentacleIndex) {
        if (!ListOfDead.Value.Contains(tentacleIndex))
            ListOfDead.Value.Add(tentacleIndex);

        _numberOfDeadTentacles++;
    }

    protected override Status OnUpdate() {
        if (!_childRunning && _numberOfDeadTentacles > 0) {
            _numberOfDeadTentacles--;
            _childRunning = true;
            StartNode(Child);
        }

        if (_childRunning) {
            var childStatus = Child.CurrentStatus;

            if (childStatus == Status.Success || childStatus == Status.Failure) {
                _childRunning = false;
            }

            return Status.Running;
        }

        return Status.Running;
    }

    protected override void OnEnd() {
        foreach (var t in _listOfTentacles) {
            int index = t.Key;
            HealthManager health = t.Value.Health;

            if (_deathHandlers.TryGetValue(index, out var handler)) {
                health.OnDeath -= handler;
            }
        }

        _deathHandlers.Clear();
        _childRunning = false;
    }
}


