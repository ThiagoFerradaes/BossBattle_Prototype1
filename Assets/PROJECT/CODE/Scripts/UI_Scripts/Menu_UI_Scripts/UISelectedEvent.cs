using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISelectedEvent : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    public void OnSelect(BaseEventData eventData) {
        OnSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData) {
        OnDeselected?.Invoke();
    }
}
