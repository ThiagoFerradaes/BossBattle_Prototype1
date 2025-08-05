using System;
using Unity.Behavior;
using UnityEngine;

[BlackboardEnum] public enum BossDifficulty { One, Two, Three, Four, Five}
public class ScripTeste : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent agent;
    [SerializeField] BossDifficulty dificulty;

    private void Start() {
        agent.SetVariableValue<Enum>("BossDifficulty", dificulty);
    }
}
