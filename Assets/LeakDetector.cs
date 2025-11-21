using UnityEngine;
using System;

public class LeakDetector : MonoBehaviour {
    public static void Check(string name, Delegate del) {
        int count = del?.GetInvocationList().Length ?? 0;
        Debug.Log($"[EVENT CHECK] {name} -> {count} inscritos");
    }
}

