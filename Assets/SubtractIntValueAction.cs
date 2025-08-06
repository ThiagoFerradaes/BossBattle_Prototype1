using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Subtract Int Value", story: "Subtract [Variable] by [Value]", category: "Action/Blackboard", id: "106ef0e8737e0f6c72e7b7d33c6199de")]
public partial class SubtractIntValueAction : Action {
    [SerializeReference] public BlackboardVariable Variable;
    [SerializeReference] public BlackboardVariable<int> Value;

    protected override Status OnStart() {
        if (Variable == null || Value == null) {
            return Status.Failure;
        }

        if (Variable is BlackboardVariable<int> intValue) {
            intValue.Value -= Value.Value;
            return Status.Success;
        }

        return Status.Failure;
    }

}

