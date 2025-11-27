using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using MyEnum;

/// <summary>
/// ScriptableObject that manages character preferences and dialogue interactions within the tavern system.
/// This class handles personality traits, environmental preferences, and dialogue system for characters.
/// </summary>
[CreateAssetMenu(fileName = "CharacterPersonality", menuName = "Characters/CharacterPersonality")]
public class CharactersSo : ScriptableObject
{
    [SerializeField] 
    [Tooltip("The character this personality profile belongs to")]
    private Character character;
    
    [Header("Personality Traits")]
    [SerializedDictionary("Environmental Characteristic", "Preference Range"), SerializeField]
    [Tooltip("Character's preferences for different environmental characteristics")]
    private SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, PreferenceRange> preferences = new();
    
    [Header("Dialogue System")]
    [SerializedDictionary("Dialogue Type", "Dialogue Data"), SerializeField] 
    [Tooltip("Character's dialogue options based on different scenarios")]
    private SerializedDictionary<DialogueSystemSo, CharacterDialogue> characterDialogues = new();

    [SerializedDictionary("Event Type", "Associated Dialogue"), SerializeField]
    [Tooltip("Specific dialogues triggered by game events")]
    private SerializedDictionary<DialogueEventEnum, DialogueSystemSo> eventDialogues = new();
    
    /// <summary>
    /// Gets the character associated with this personality profile.
    /// </summary>
    public Character Character => character;

    /// <summary>
    /// Retrieves the dialogue associated with a specific event.
    /// </summary>
    /// <param name="eventToTrigger">The event that triggers the dialogue</param>
    /// <returns>The dialogue system associated with the event, or null if none exists</returns>
    public DialogueSystemSo DialogueEvents(DialogueEventEnum eventToTrigger) =>
        eventDialogues.GetValueOrDefault(eventToTrigger);
    
    /// <summary>
    /// Evaluates a character's preference level for a given environmental characteristic value.
    /// </summary>
    /// <param name="characteristic">The environmental characteristic to evaluate</param>
    /// <param name="value">The value to assess against the preference range</param>
    /// <returns>Preference score ranging from -1 (strong dislike) to 1 (strong like)</returns>
    public float CalculatePreference(TypeOfEnvironmentCharacteristicEnum characteristic, float value)
    {
        if (!preferences.TryGetValue(characteristic, out var preference)) return 0;
        
        // Check if value falls within preferred range
        if (value >= preference.range.x && value <= preference.range.y)
        {
            if (preference.isExtreme) return preference.importance;
            
            float midValue = (preference.range.x + preference.range.y) * 0.5f;
            float halfRange = (preference.range.y - preference.range.x) * 0.5f;
            float normalizedValue = (1f - math.abs(value - midValue) / halfRange) * preference.importance;
            
            return float.IsNaN(normalizedValue) ? preference.importance : normalizedValue;
        }

        if (preference.isExtreme) return -preference.importance;
        
        // Calculate negative preference for values outside range
        if (value < preference.range.x)
        {
            return -math.saturate((preference.range.x - value) / preference.range.x) * preference.importance;
        }

        return value > preference.range.y ? 
            -math.saturate((value - preference.range.y) / (100 - preference.range.y)) * preference.importance : 0;
    }

    /// <summary>
    /// Calculates the total preference score based on environmental characteristics from furniture features.
    /// </summary>
    /// <param name="environmentCharacteristics">List of furniture features with their environmental characteristics</param>
    /// <returns>Total preference score combining all environmental factors</returns>
    public float CalculateAllPreferences(List<FurnitureFeaturesSo> environmentCharacteristics)
    {
        var environmentCharacteristic = new Dictionary<TypeOfEnvironmentCharacteristicEnum, float>();

        // Aggregate all characteristic values from furniture features
        foreach (var characteristic in environmentCharacteristics)
        {
            var characteristics = characteristic.GetAllCharacteristics();
            foreach (var (key, value) in characteristics)
            {
                if (environmentCharacteristic.ContainsKey(key))
                {
                    environmentCharacteristic[key] += value.value;
                }
                else
                {
                    environmentCharacteristic.Add(key, value.value);
                }
            }
        }
        
        // Calculate the total preference score for all characteristics
        return preferences.Sum(preference => 
            environmentCharacteristic.TryGetValue(preference.Key, out var value) 
                ? CalculatePreference(preference.Key, value) 
                : CalculatePreference(preference.Key, 0));
    }

    /// <summary>
    /// Calculates the preference difference when removing or modifying an environmental characteristic.
    /// </summary>
    /// <param name="characteristic">The environmental characteristic being modified</param>
    /// <param name="beginnerValue">Original value of the characteristic</param>
    /// <param name="alterValue">New value after modification</param>
    /// <param name="value"></param>
    /// <returns>Absolute difference in preference between original and modified values</returns>
    public float CalculateRemove(TypeOfEnvironmentCharacteristicEnum characteristic,float beginnerValue ,float alterValue, float value)
    {
        value -= CalculatePreference(characteristic, beginnerValue); //reset original value
        value += CalculatePreference(characteristic, alterValue); //add new value
        
        return value;
    }
    
    /// <summary>
    /// Retrieves the most appropriate dialogue based on the current friendship level.
    /// </summary>
    /// <param name="friendshipLevel">Current relationship level with the character</param>
    /// <param name="availableDialogues">Collection of available dialogue options</param>
    /// <returns>The most suitable dialogue system, or null if none available</returns>
    public DialogueSystemSo GetDialogueForFriendshipLevel(float friendshipLevel, 
        Dictionary<DialogueSystemSo, CharacterDialogue> availableDialogues) =>
        availableDialogues
            .Where(d => d.Value.minimumFriendshipLevel <= friendshipLevel && !d.Value.hasOccurred)
            .Select(d => d.Key)
            .FirstOrDefault();
    
    /// <summary>
    /// Returns a mutable copy of character dialogues.
    /// </summary>
    /// <returns>Dictionary containing dialogue systems and their associated data</returns>
    public Dictionary<DialogueSystemSo, CharacterDialogue> Dialogues() => new(characterDialogues);

    /// <summary>
    /// Returns a mutable copy of character preferences.
    /// </summary>
    /// <returns>Dictionary containing environmental characteristics and their preference ranges</returns>
    public Dictionary<TypeOfEnvironmentCharacteristicEnum, PreferenceRange> Preferences() => new(preferences);
}

[Serializable]
public class PreferenceRange
{
    [Tooltip("Indicates if this preference has extreme impact on character's mood")]
    public bool isExtreme;
    [Tooltip("Weighting factor for this preference (0-1)")]
    public float importance;
    [Tooltip("Acceptable value range (x: minimum, y: maximum)")]
    [MinMaxSlider(0,100)]
    public Vector2 range;
}

[Serializable]
public class CharacterDialogue
{
    [Tooltip("Required friendship level to unlock this dialogue")]
    public float minimumFriendshipLevel;
    [HideInInspector] public bool hasOccurred;
}