using System;
using System.IO;
using UnityEngine;
using MyEnum;

/// <summary>
/// ScriptableObject that manages game configuration settings including language preferences,
/// pre-casting options, and dash movement settings.
/// </summary>
[CreateAssetMenu(fileName = "ConfigurationSO", menuName = "Scriptable Objects/ConfigurationSO")]
public class ConfigurationSo : ScriptableObject
{
    [SerializeField]
    [Tooltip("Determines if pre-casting functionality is enabled")]
    private bool preCastOn;

    [SerializeField] 
    [Tooltip("Determines if dash moves towards mouse position")]
    private bool dashToMouse = true;

    [SerializeField]
    [Tooltip("Current language setting for the game")]
    private EnumLanguage language;

    /// <summary>
    /// Event triggered when the language setting is changed
    /// </summary>
    public event Action<EnumLanguage> OnLanguageChanged;

    [Serializable]
    private struct SaveData
    {
        public bool preCastOn;
        public bool dashToMouse; 
        public EnumLanguage language;
    }
    
    private static string SavePath => Path.Combine(Application.persistentDataPath, "config.json");

    /// <summary>
    /// Sets the game language and triggers the language change event if the new language is different
    /// </summary>
    /// <param name="newLanguage">The new language to set</param>
    public void SetLanguage(EnumLanguage newLanguage)
    {
        if (language == newLanguage) return;
        language = newLanguage;
        OnLanguageChanged?.Invoke(language);
    }

    /// <summary>
    /// Gets the current language setting
    /// </summary>
    /// <returns>The current language enum value</returns>
    public EnumLanguage GetLanguage() => language;

    /// <summary>
    /// Sets the pre-cast setting
    /// </summary>
    /// <param name="newPreCast">The new pre-cast value to set</param>
    public void SetPreCast(bool newPreCast) => preCastOn = newPreCast;

    /// <summary>
    /// Gets the current pre-cast setting
    /// </summary>
    /// <returns>The current pre-cast value</returns>
    public bool GetPreCast() => preCastOn;

    /// <summary>
    /// Sets whether dash should move towards the mouse position
    /// </summary>
    /// <param name="newDashToMouse">The new dash-to-mouse value to set</param>
    public void SetDashToMouse(bool newDashToMouse) => dashToMouse = newDashToMouse; 

    /// <summary>
    /// Gets the current dash-to-mouse setting
    /// </summary>
    /// <returns>The current dash-to-mouse value</returns>
    public bool GetDashToMouse() => dashToMouse;

    /// <summary>
    /// Loads configuration settings from the JSON file. Creates a default config if a file doesn't exist.
    /// </summary>
    public void LoadFromJson()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                SaveToJson();
                return;
            }

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);

            preCastOn = data.preCastOn;
            dashToMouse = data.dashToMouse;
            SetLanguage(data.language);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading configuration: {e.Message}");
            SaveToJson(); // Create new config files with defaults
        }
    }

    /// <summary>
    /// Saves current configuration settings to JSON file
    /// </summary>
    public void SaveToJson()
    {
        try
        {
            var data = new SaveData
            {
                preCastOn = preCastOn,
                dashToMouse = dashToMouse,
                language = language
            };

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving configuration: {e.Message}"); 
        }
    }
}