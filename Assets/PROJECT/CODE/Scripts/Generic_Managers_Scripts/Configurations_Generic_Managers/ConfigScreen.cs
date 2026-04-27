using UnityEngine;
using UnityEngine.EventSystems;

public class ConfigScreen : MonoBehaviour {
    [SerializeField] protected GameObject screen;
    [SerializeField] protected GameObject firstButtonSelected;
    public virtual void HandleConfigurationScreen(bool isOn) {
        screen.SetActive(isOn);

        if (isOn) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonSelected);
        }
    }
}
