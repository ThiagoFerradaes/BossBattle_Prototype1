using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// ScriptableObject that manages text content in different languages.
/// Used for localization and text management in the game.
/// </summary>
[CreateAssetMenu(fileName = "TextBoxes", menuName = "Texts/TextBoxes")]
public class TextBoxesSo : ScriptableObject
{
    /// <summary>
    /// Dictionary storing text content for different languages.
    /// Key: Language enum value
    /// TValue: Corresponding text content
    /// </summary>
    [FormerlySerializedAs("_text")]
    [SerializedDictionary("Language", "Text"), SerializeField] 
    [Tooltip("Dictionary containing text content for different languages")]
    private SerializedDictionary<EnumLanguage, string> text = new();

    /// <summary>
    /// Retrieves the text content for the specified language
    /// </summary>
    /// <param name="language">The language to get the text for</param>
    /// <returns>The text content in the specified language</returns>
    public string GetText(EnumLanguage language)
    {
        if (text.ContainsKey(language)) return text[language];
        Debug.LogWarning($"No text found for language {language} and {text.Count} languages available.");
        return string.Empty;
    }
 
    #if UNITY_EDITOR
    /// <summary>
    /// Editor-only method to access the text dictionary.
    /// Used by the custom editor for text management.
    /// </summary>
    /// <returns>The dictionary containing all language-text pairs</returns>
    public SerializedDictionary<EnumLanguage, string> GetDictionary() => text;
    #endif
}