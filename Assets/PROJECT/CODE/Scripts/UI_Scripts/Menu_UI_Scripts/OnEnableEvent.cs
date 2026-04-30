using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onEnableEvent;
    [SerializeField] UnityEvent onDisableEvent;

    private void OnEnable() {
        onEnableEvent?.Invoke();
    }
    private void OnDisable() {
        onDisableEvent?.Invoke();
    }
}
