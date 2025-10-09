using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextBoxes", menuName = "Texts/TextBoxes")]
public class TextBoxes : ScriptableObject
{
    [Tooltip("Dictionary containing different Text of Language")]
    private readonly Dictionary<EnumLanguage, string> _text = new();

    public string GetText(EnumLanguage language)
    {
        return _text[language];
    }
 
#if UNITY_EDITOR
    public Dictionary<EnumLanguage, string> GetDictionary() => _text;
#endif
    
}
