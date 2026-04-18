using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public enum TypesOfAudio {
    Global,
    Music,
    SFX,
    Ambient
}
public class AudioConfig : ConfigScreen {

    [Header("Sliders")]
    [SerializeField, SerializedDictionary("Type", "Slider")] SerializedDictionary<TypesOfAudio, Slider> audioSliders;


    [Header("RTPCs")]
    [SerializeField, SerializedDictionary("Type", "RTPCs")] SerializedDictionary<TypesOfAudio, AK.Wwise.RTPC> RTPCs;


    private void Start() {
        SetInitialSliderValues();
        SetSlidersFunctions();
    }

    void SetInitialSliderValues() {
        foreach (var pair in audioSliders) {
            float volumeValue = ConfigurationWhiteBoard.Instance.AudioValues[pair.Key];
            pair.Value.value = volumeValue;

            SetVolume(volumeValue, pair.Key);
        }
    }

    void SetSlidersFunctions() {
        foreach (var slider in audioSliders) {
            slider.Value.onValueChanged.AddListener((value) => SetVolume(value, slider.Key));
        }
    }

    void SetVolume(float volumeValue, TypesOfAudio type) {

        ConfigurationWhiteBoard.Instance.AudioValues[type] = volumeValue;

        if (RTPCs.ContainsKey(type))
            RTPCs[type].SetGlobalValue(volumeValue);
    }
}
