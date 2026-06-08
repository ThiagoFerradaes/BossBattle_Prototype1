using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VideoConfig : ConfigScreen {

    [Header("Resolution Settings")]
    [SerializeField] TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    List<Resolution> filteredResolutions = new();
    List<string> resolutionOptions = new();

    float currentRefreshRate;

    [Header("Graphics Settings")]
    [SerializeField] TMP_Dropdown graphicsDropdown;
    [SerializeField] List<LocalizedString> graphicsOptions;
    List<string> graphicsOptionsString = new();

    [Header("Window mode settings")]
    [SerializeField] TMP_Dropdown windowModeDropdown;
    [SerializeField] List<LocalizedString> windowModeOptions;
    List<string> windowModeOptionsString = new();

    [Header("Fps Settings")]
    [SerializeField] TMP_Dropdown fpsDropdown;
    [SerializeField] int[] fpsOptions;

    Locale _currentLocale;

    private void Start() {
        SetInitialResolutionOptions();
        SetGraphicDropdownValues();
        SetWindowModeDropdownValues();
        SetFpsDropdownValues();
        SetCurrentLocale();
    }

    void SetCurrentLocale() {
        _currentLocale = LocalizationSettings.SelectedLocale;
    }

    public override void HandleConfigurationScreen(bool isOn) {
        if (isOn && _currentLocale != LocalizationSettings.SelectedLocale) {
            RefreshText();
            SetCurrentLocale();
        }
        base.HandleConfigurationScreen(isOn);
    }

    void RefreshText() {
        SetWindowModeDropdownValues();
        SetGraphicDropdownValues();
    }


    #region Resolution Settings

    /// <summary>
    /// Settando o dropdown
    /// </summary>
    void SetInitialResolutionOptions() {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++) {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate) {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        for (int i = 0; i < filteredResolutions.Count; i++) {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            resolutionOptions.Add(resolutionOption);
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.value = resolutionOptions.IndexOf(ConfigurationWhiteBoard.Instance.ResolutionWidth + "x" + ConfigurationWhiteBoard.Instance.ResolutionHeight);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    /// <summary>
    /// Método do botão do dropdown
    /// </summary>
    /// <param name="resolutionIndex"></param>
    void SetResolution(int resolutionIndex) {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        ConfigurationWhiteBoard.Instance.ResolutionWidth = resolution.width;
        ConfigurationWhiteBoard.Instance.ResolutionHeight = resolution.height;
    }
    #endregion

    #region Graphic Settings
    /// <summary>
    /// Settando o dropdown
    /// </summary>
    void SetGraphicDropdownValues() {

        graphicsOptionsString.Clear();

        for (int i = 0; i < graphicsOptions.Count; i++) {
            graphicsOptionsString.Add(graphicsOptions[i].GetLocalizedString());
        }

        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(graphicsOptionsString);

        graphicsDropdown.value = ConfigurationWhiteBoard.Instance.GraphicQualityIndex;
        graphicsDropdown.onValueChanged.RemoveAllListeners();
        graphicsDropdown.onValueChanged.AddListener(SetGraphicsQualityIndex);
    }

    /// <summary>
    /// Método do botão do dropdown
    /// </summary>
    /// <param name="index"></param>
    void SetGraphicsQualityIndex(int index) {
        // Aqui a gente só faz no whiteboard pq a qualidade só é aplicada no começo do jogo
        ConfigurationWhiteBoard.Instance.GraphicQualityIndex = index;
    }

    #endregion

    #region Window Mode Settings
    /// <summary>
    /// Settando o dropdown
    /// </summary>
    void SetWindowModeDropdownValues() {

        windowModeOptionsString.Clear();
        for (int i = 0; i < windowModeOptions.Count; i++) {
            windowModeOptionsString.Add(windowModeOptions[i].GetLocalizedString());
        }

        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(windowModeOptionsString);

        windowModeDropdown.value = ConfigurationWhiteBoard.Instance.WindowModeIndex;
        windowModeDropdown.onValueChanged.RemoveAllListeners();
        windowModeDropdown.onValueChanged.AddListener(SetWindowMode);
    }

    /// <summary>
    /// Método do botão do dropdown
    /// </summary>
    /// <param name="windowIndex"></param>
    void SetWindowMode(int windowIndex) {
        switch (windowIndex) {
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

        ConfigurationWhiteBoard.Instance.WindowModeIndex = windowIndex;
    }

    #endregion

    #region Fps Settings

    /// <summary>
    /// Settando o Dropdown
    /// </summary>
    void SetFpsDropdownValues() {

        fpsDropdown.ClearOptions();

        List<string> options = new();
        for (int i = 0; i < fpsOptions.Length; i++) {
            string fpsOption = fpsOptions[i] + " FPS";
            options.Add(fpsOption);
        }

        fpsDropdown.AddOptions(options);
        fpsDropdown.value = System.Array.IndexOf(fpsOptions, ConfigurationWhiteBoard.Instance.FPSValue);
        fpsDropdown.onValueChanged.AddListener(SetFps);
    }

    /// <summary>
    /// Método do botão do dropdown
    /// </summary>
    /// <param name="fpsIndex"></param>
    void SetFps(int fpsIndex) {
        Application.targetFrameRate = fpsOptions[fpsIndex];
        ConfigurationWhiteBoard.Instance.FPSValue = fpsOptions[fpsIndex];
    }

    #endregion
}
