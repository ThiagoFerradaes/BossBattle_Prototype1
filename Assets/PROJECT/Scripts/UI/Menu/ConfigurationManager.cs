using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [SerializeField] Toggle preCastOnToggle;
    [SerializeField] Toggle dashToMouseToggle;
    [SerializeField] TMP_Dropdown  languageDropdown;
    
    [SerializeField] ConfigurationSo configurationSo;
    
    private void Start() {
        SettingToggles();
    }

    async void SettingToggles()
    {
        try
        {
            //preCastOnToggle.isOn = ConfigurationWhiteBoard.Instance.PreCastOn;
            preCastOnToggle.onValueChanged.AddListener(PreCastToggle);

            //dashToMouseToggle.isOn = ConfigurationWhiteBoard.Instance.DashToMouse;
            dashToMouseToggle.onValueChanged.AddListener(DashToMouseToggle);
        
            languageDropdown.options = Languages();
            languageDropdown.onValueChanged.AddListener(LanguageChange);
            
            await LoadConfiguration();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    private Task LoadConfiguration()
    {
        var getPreCast = configurationSo.GetPreCast();
        var getDashToMouse = configurationSo.GetDashToMouse();
        var language = configurationSo.GetLanguage();
        
        Debug.Log($"{language} and ID {(int)language}");
        
        preCastOnToggle.isOn = getPreCast;
        dashToMouseToggle.isOn = getDashToMouse;
        languageDropdown.value = (int)language;

        return Task.CompletedTask;
    }
    
    private List<TMP_Dropdown.OptionData> Languages()
    {
        return (from EnumLanguage language in Enum.GetValues(typeof(EnumLanguage)) select new TMP_Dropdown.OptionData(language.ToString())).ToList();
    }

    private void LanguageChange(int index)
    {
        configurationSo.SetLanguage((EnumLanguage)index);
    }
    
    void PreCastToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.PreCastOn = newValue;
        configurationSo.SetPreCast(newValue);
    }
    void DashToMouseToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.DashToMouse = newValue;
        configurationSo.SetDashToMouse(newValue);
    }
}
