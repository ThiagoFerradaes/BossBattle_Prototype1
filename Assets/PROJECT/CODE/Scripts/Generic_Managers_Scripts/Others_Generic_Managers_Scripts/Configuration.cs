using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public enum ConfigurationScreen {
    Gameplay,
    Graphics,
    Audio,
    Language
}
public class Configuration : MonoBehaviour {

    [Space(10)]

    [SerializeField, Foldout("Components")] LocalizeStringEvent screenTitle;

    [Space(10)]

    [SerializeField, Foldout("Screens")] GameObject configurationScreen;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " Gameobject")] SerializedDictionary<ConfigurationScreen, GameObject> screens;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " String")] SerializedDictionary<ConfigurationScreen, LocalizedString> screensTitles;

    [Space(10)]

    [SerializeField, Foldout("Toggles")] Toggle dashToMouseToggle;
    [SerializeField, Foldout("Toggles")] Toggle ptToggle;
    [SerializeField, Foldout("Toggles")] Toggle enToggle;

    [Space(10)]

    [SerializeField, Foldout("Buttons")] Button closeConfigurationScreenButton;
    [SerializeField, Foldout("Buttons"), SerializedDictionary("Type of Screen", " Button")] SerializedDictionary<ConfigurationScreen, Button> screenButtons;

    [Space(10)]

    [SerializeField, Foldout("Language Codes")] string ptLanguageCode = "pt-BR";
    [SerializeField, Foldout("Language Codes")] string enLanguageCode = "en";

    Dictionary<string, Toggle> languageToggles;

    #region Awake and Setup

    private void Awake() {
        SetButtonsFunctions();
        SetDictionary();
        configurationScreen.SetActive(false);
    }

    void SetButtonsFunctions() {
        closeConfigurationScreenButton.onClick.AddListener(() => {
            CloseConfigurationScreen();
        });

        foreach (var button in screenButtons) {
            var screenType = button.Key;
            button.Value.onClick.AddListener(() => TurnScreenOn(screenType));
        }
    }

    void SetDictionary() {
        languageToggles = new Dictionary<string, Toggle> {
            { ptLanguageCode, ptToggle },
            { enLanguageCode, enToggle }
        };
    }

    private void Start() {
        SetInitialToggleValues();

        SetToggleFunctions();
    }

    void SetInitialToggleValues() {
        dashToMouseToggle.isOn = ConfigurationWhiteBoard.Instance.DashToMouse;
        ptToggle.isOn = LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale(ptLanguageCode);
        enToggle.isOn = LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale(enLanguageCode);
    }

    void SetToggleFunctions() {
        ptToggle.onValueChanged.AddListener((isOn) => {
            if (isOn) {
                SetLanguage(ptLanguageCode);
            }
        });

        enToggle.onValueChanged.AddListener((isOn) => {
            if (isOn) {
                SetLanguage(enLanguageCode);
            }
        });

        dashToMouseToggle.onValueChanged.AddListener((isOn) => {
            ConfigurationWhiteBoard.Instance.DashToMouse = isOn;
        });
    }

    void CloseConfigurationScreen() {
        configurationScreen.SetActive(false);
    }
    #endregion

    #region Initialization

    public void InitializeConfigurationScreen() {
        configurationScreen.SetActive(true);
        TurnScreenOn(ConfigurationScreen.Gameplay);
    }

    #endregion

    #region GenericScreens

    void TurnScreenOn(ConfigurationScreen screenType) {
        foreach (var screen in screens) {
            screen.Value.SetActive(screen.Key == screenType);
        }

        screenTitle.StringReference = screensTitles[screenType];
    }

    #endregion

    #region LanguageScreen

    void SetLanguage(string languageIndex) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageIndex);

        foreach (var toggle in languageToggles.Values) {
            toggle.isOn = languageToggles[languageIndex] == toggle;
            toggle.interactable = languageToggles[languageIndex] != toggle;
        }
    }

    #endregion




}
