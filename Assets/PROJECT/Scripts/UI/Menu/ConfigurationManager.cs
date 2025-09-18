using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [SerializeField] Toggle preCastOnToggle;
    [SerializeField] Toggle dashToMouseToggle;

    private void Start() {
        SettingToggles();
    }

    void SettingToggles() {
        preCastOnToggle.isOn = ConfigurationWhiteBoard.Instance.PreCastOn;
        preCastOnToggle.onValueChanged.AddListener(PreCastToggle);

        dashToMouseToggle.isOn = ConfigurationWhiteBoard.Instance.DashToMouse;
        dashToMouseToggle.onValueChanged.AddListener(DashToMouseToggle);
    }
    void PreCastToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.PreCastOn = newValue;
    }
    void DashToMouseToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.DashToMouse = newValue;
    }
}
