using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Repeat X",
    description: "Repeats the flow underneath a specified number of times.",
    story: "Repeat [X] amount",
    category: "Flow",
    id: "db5f4b064fe46cf9c6d716f94eb4ff3c")]
public partial class RepeatXModifier : Modifier {
    [SerializeReference] public BlackboardVariable<int> X;
    private int _currentCount;
    protected override Status OnStart() {
        _currentCount = 0;

        if (Child == null || X == null || X.Value <= 0) {
            return Status.Failure;
        }

        return StartNode(Child);
    }

    protected override Status OnUpdate() {
        var childStatus = Child.CurrentStatus;

        if (childStatus == Status.Running || childStatus == Status.Waiting) {
            return Status.Running;
        }

        _currentCount++;

        if (_currentCount >= X.Value) {
            return Status.Success;
        }

        EndNode(Child);
        return StartNode(Child);
    }

}

