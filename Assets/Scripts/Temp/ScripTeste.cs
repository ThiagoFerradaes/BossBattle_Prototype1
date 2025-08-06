using System;
using Unity.Behavior;
using UnityEngine;

[BlackboardEnum] public enum BossDifficulty { One, Two, Three, Four, Five}
public class ScripTeste : MonoBehaviour
{
    BehaviorGraphAgent agent;
    [SerializeField] BossDifficulty dificulty;

    private void Awake() {
        agent = GetComponent<BehaviorGraphAgent>();
    }
    private void Start() {
        agent.SetVariableValue<Enum>("BossDifficulty", dificulty);
    }
}
