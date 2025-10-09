using TMPro;
using UnityEngine;

/// <summary>
/// Component responsible for localizing UI text elements based on the selected language
/// </summary>
public class UITextLocalizer : MonoBehaviour
{
    /// <summary>
    /// Reference to the game configuration
    /// </summary>
    private ConfigurationSo _config;
    
    /// <summary>
    /// Reference to the TextBoxes asset containing localized text content
    /// </summary>
    [SerializeField] 
    [Tooltip("TextBoxes asset containing the localized text content")]
    private TextBoxes textBox;
    
    /// <summary>
    /// Reference to the TextMeshPro UI component that will display the text
    /// </summary>
    [SerializeField]
    [Tooltip("TextMeshPro component where the text will be displayed")]
    private TMP_Text textUI;
    
    /// <summary>
    /// Called when the component is enabled
    /// Initializes configuration and sets up language change listener
    /// </summary>
    private void OnEnable()
    {
        _config = GameConfig.Config;
        
        if (_config == null) return;
        
        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }

    /// <summary>
    /// Called when the component is disabled
    /// Removes the language change listener
    /// </summary>
    private void OnDisable()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    /// <summary>
    /// Updates the UI text based on the selected language
    /// </summary>
    /// <param name="lang">The language to display the text in</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        if (textBox == null || textUI == null)
            return;

        textUI.text = textBox.GetText(lang);
    }
}