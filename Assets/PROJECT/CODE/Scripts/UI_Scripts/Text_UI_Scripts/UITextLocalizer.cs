using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using MyEnum;
using UnityEngine;

/// <summary>
/// Component responsible for managing UI text localization based on the selected language.
/// This class handles both single and multiple text elements through different operating modes.
/// </summary>
public class UITextLocalizer : MonoBehaviour
{
    [Header("Mode Settings")]
    [Tooltip("Enable to handle multiple text boxes simultaneously (List Mode)")]
    [SerializeField] private bool useListMode;
    
    /// <summary>
    /// Reference to the game's configuration settings
    /// </summary>
    private ConfigurationSo _config;
    
    [SerializeField] 
    [Tooltip("Single text box configuration for basic mode")]
    private Text textBox;
    
    [Tooltip("Collection of text boxes for handling multiple UI elements")]
    [SerializeField] private List<Text> textBoxesList = new();
    
    /// <summary>
    /// Initializes the component and sets up language change handling when enabled.
    /// Subscribes to language change events and performs initial text update.
    /// </summary>
    private void OnEnable()
    {
        _config = GameConfig.Config;
        
        if (_config == null) return;
        
        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }

    /// <summary>
    /// Performs cleanup by unsubscribing from language change events when disabled.
    /// </summary>
    private void OnDisable()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    public void SetTexBox(TextBoxesSo textBoxesSo)
    {
        if(textBoxesSo is null) return;
        textBox.textBoxes = textBoxesSo;
        if(_config is null) return;
        UpdateLanguage(_config.GetLanguage());
    }
    
    /// <summary>
    /// Updates the UI text content based on the selected language.
    /// Handles both single text box and list mode configurations.
    /// </summary>
    /// <param name="lang">The target language for text localization</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        if (!useListMode)
        {
            if (textBox.uiText == null || textBox.textBoxes == null)
                return;

            textBox.uiText.text = textBox.textBoxes.GetText(lang);
        }
        else
        {
            foreach (var t in textBoxesList.Where(t => t.uiText != null && t.textBoxes != null))
            {
                t.uiText.text = t.textBoxes.GetText(lang);
            }
        }
    }
}

/// <summary>
/// Represents a text element configuration containing UI and localization data.
/// </summary>
[Serializable]
public class Text
{
    /// <summary>
    /// Reference to the TextMeshPro UI component
    /// </summary>
    public TMP_Text uiText;
    
    /// <summary>
    /// Reference to the localized text content container
    /// </summary>
    public TextBoxesSo textBoxes;
}