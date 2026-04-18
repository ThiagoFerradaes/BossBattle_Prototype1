using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ConfigurationWhiteBoard : MonoBehaviour
{
    public static ConfigurationWhiteBoard Instance;

    [Space(10)]

    [Header("Gameplay")]
    public bool PreCastOn = false;
    public bool DashToMouse = true;

    [Space(10)]

    [Header("Graphics")]
    public int GraphicQualityIndex = 2;
    public int FPSValue = 60;
    public int ResolutionHeight = 1080;
    public int ResolutionWidth = 1920;
    public int WindowModeIndex = 2;

    [Header("Language")]
    public string LanguageCode = "pt-BR";

    [Header("Audio")]
    [SerializedDictionary("Type", "Volume")] public SerializedDictionary<TypesOfAudio, float> AudioValues;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
        SetGraphicsConfigurations();
        SetLanguage();
    }

    void SetGraphicsConfigurations() {
        // Quality
        QualitySettings.SetQualityLevel(GraphicQualityIndex);

        // FPS
        Application.targetFrameRate = FPSValue;

        // Window Mode
        switch (WindowModeIndex) {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        // Resolution
        Screen.SetResolution(ResolutionWidth, ResolutionHeight, Screen.fullScreenMode);
    }

    void SetLanguage() {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(LanguageCode);
    }

}
