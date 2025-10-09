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
    
    private ConfigurationSo _configurationSo;
    
    private void Start()
    {
        _configurationSo = GameConfig.Config;
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
        
        
        var getPreCast = _configurationSo.GetPreCast();
        var getDashToMouse = _configurationSo.GetDashToMouse();
        var language = _configurationSo.GetLanguage();
        
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
        _configurationSo.SetLanguage((EnumLanguage)index);
    }
    
    void PreCastToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.PreCastOn = newValue;
        _configurationSo.SetPreCast(newValue);
    }
    void DashToMouseToggle(bool newValue) {
        ConfigurationWhiteBoard.Instance.DashToMouse = newValue;
        _configurationSo.SetDashToMouse(newValue);
    }
}
