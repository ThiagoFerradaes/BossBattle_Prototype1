using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageConfig : ConfigScreen {

    [SerializeField, Foldout("Toggles")] Toggle ptToggle;
    [SerializeField, Foldout("Toggles")] Toggle enToggle;

    [SerializeField, Foldout("Language Codes")] string ptLanguageCode = "pt-BR";
    [SerializeField, Foldout("Language Codes")] string enLanguageCode = "en";

    Dictionary<string, Toggle> languageToggles;

    private void Awake() {
        SetDictionary();
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
    }

    void SetLanguage(string languageIndex) {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageIndex);

        foreach (var toggle in languageToggles.Values) {
            toggle.isOn = languageToggles[languageIndex] == toggle;
            toggle.interactable = languageToggles[languageIndex] != toggle;
        }

        ConfigurationWhiteBoard.Instance.LanguageCode = languageIndex;
    }
}
