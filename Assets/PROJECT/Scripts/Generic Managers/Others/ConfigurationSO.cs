using UnityEngine;

[CreateAssetMenu(fileName = "ConfigurationSO", menuName = "Scriptable Objects/ConfigurationSO")]
public class ConfigurationSo : ScriptableObject
{

    [SerializeField] private bool preCastOn;
    [SerializeField] private bool dashToMouse = true;

    [SerializeField] private EnumLanguage language;
    
    
    public void SetLanguage(EnumLanguage newLanguage)
    {
        language = newLanguage;
    }
    
    public EnumLanguage GetLanguage()
    {
        return language;
    }

    public void SetPreCast(bool newPreCast)
    {
        preCastOn = newPreCast;
    }
    
    public bool GetPreCast()
    {
        return preCastOn;
    }

    public void SetDashToMouse(bool newDashToMouse)
    {
        dashToMouse = newDashToMouse;
    }
    
    public bool GetDashToMouse()
    {
        return dashToMouse;   
    }
}
