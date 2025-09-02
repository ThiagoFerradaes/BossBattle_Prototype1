using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [SerializeField] Toggle preCastOnToggle;

    private void Start() {
        preCastOnToggle.isOn = ConfigurationWhiteBoard.Instance.PreCastOn;
        preCastOnToggle.onValueChanged.AddListener(PreCastToggle);
    }

    void PreCastToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.PreCastOn = newValue;
    }
}
