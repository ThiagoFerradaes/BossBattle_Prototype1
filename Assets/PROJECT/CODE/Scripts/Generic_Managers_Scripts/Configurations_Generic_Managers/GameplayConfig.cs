using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GameplayConfig : ConfigScreen {

    [SerializeField, Foldout("Toggles")] Toggle dashToMouseToggle;


    private void Start() {
        SetInitialToggleValues();

        SetToggleFunctions();
    }
    void SetInitialToggleValues() {
        dashToMouseToggle.isOn = ConfigurationWhiteBoard.Instance.DashToMouse;
    }

    void SetToggleFunctions() {
        dashToMouseToggle.onValueChanged.AddListener((isOn) => {
            ConfigurationWhiteBoard.Instance.DashToMouse = isOn;
        });
    }
}
