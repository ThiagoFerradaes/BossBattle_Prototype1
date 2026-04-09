using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] Toggle dashToMouseToggle;
    [SerializeField] Toggle ptToggle;
    [SerializeField] Toggle enToggle;

    [Header("Language Codes")]
    [SerializeField] string ptLanguageCode = "pt-BR";
    [SerializeField] string enLanguageCode = "en";

    Dictionary<string, Toggle> languageToggles;


    private void Awake()
    {
        SetInitialToggleValues();
        SetDictionary();
        SetToggleFunctions();

    }

    void SetDictionary()
    {
        languageToggles = new Dictionary<string, Toggle> {
            { ptLanguageCode, ptToggle },
            { enLanguageCode, enToggle }
        };
    }
    void SetInitialToggleValues()
    {
        dashToMouseToggle.isOn = false;
        ptToggle.isOn = LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale(ptLanguageCode);
        enToggle.isOn = LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.GetLocale(enLanguageCode);
    }

    void SetToggleFunctions()
    {
        ptToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                SetLanguage(ptLanguageCode);
            }
    });

        enToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                SetLanguage(enLanguageCode);
            }
        });
    }

    void SetLanguage(string languageIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageIndex);

        foreach (var toggle in languageToggles.Values)
        {
            toggle.isOn = languageToggles[languageIndex] == toggle;
            toggle.interactable = languageToggles[languageIndex] != toggle;
        }
    }
}
