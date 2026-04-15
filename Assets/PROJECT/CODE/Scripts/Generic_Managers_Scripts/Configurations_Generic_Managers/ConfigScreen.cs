using UnityEngine;

public class ConfigScreen : MonoBehaviour {
    [SerializeField] protected GameObject screen;
    public virtual void HandleConfigurationScreen(bool isOn) {
        screen.SetActive(isOn);
    }
}
