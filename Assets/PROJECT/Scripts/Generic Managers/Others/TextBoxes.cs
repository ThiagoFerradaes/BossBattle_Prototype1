using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that manages text content in different languages.
/// Used for localization and text management in the game.
/// </summary>
[CreateAssetMenu(fileName = "TextBoxes", menuName = "Texts/TextBoxes")]
public class TextBoxes : ScriptableObject
{
    /// <summary>
    /// Dictionary storing text content for different languages.
    /// Key: Language enum value
    /// TValue: Corresponding text content
    /// </summary>
    [Tooltip("Dictionary containing text content for different languages")]
    private readonly Dictionary<EnumLanguage, string> _text = new();

    /// <summary>
    /// Retrieves the text content for the specified language
    /// </summary>
    /// <param name="language">The language to get the text for</param>
    /// <returns>The text content in the specified language</returns>
    public string GetText(EnumLanguage language)
    {
        return _text[language];
    }
 
    #if UNITY_EDITOR
    /// <summary>
    /// Editor-only method to access the text dictionary.
    /// Used by the custom editor for text management.
    /// </summary>
    /// <returns>The dictionary containing all language-text pairs</returns>
    public Dictionary<EnumLanguage, string> GetDictionary() => _text;
    #endif
}