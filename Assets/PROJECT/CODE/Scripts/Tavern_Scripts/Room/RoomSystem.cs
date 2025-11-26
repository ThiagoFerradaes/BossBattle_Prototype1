using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using MyEnum;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages the room furniture system, including furniture placement, unlocking, and UI display.
/// Handles furniture inventory, room size management, and localization for tavern rooms.
/// </summary>
public class RoomSystem : MonoBehaviour
{
    #region Inspector Fields

    [Header("Furniture Configuration")]
    [SerializeField]
    [Tooltip("List of furniture currently placed in the room")]
    private List<Furniture> listOfFurniture = new List<Furniture>();

    [SerializeField]
    [Tooltip("Character associated with this room")]
    private CharactersSo character;

    [SerializeField]
    [Tooltip("Array of available furniture slots in the room")]
    private SlotFurnitureRoom[] slotFurnitureRooms;

    [SerializeField]
    [Tooltip("Current number of active furniture slots")]
    private byte numberOfFurniture;

    [Header("UI References")]
    [SerializeField]
    [Tooltip("Text component displaying furniture name")]
    private TMP_Text nameFurniture;

    [SerializeField]
    [Tooltip("Text component displaying furniture description")]
    private TMP_Text descriptionFurniture;

    [FormerlySerializedAs("RoomAtributes")]
    [Space(10)]
    [Header("Attributes")]
    [SerializeField]
    [Tooltip("Attributes of the room affecting character preferences")]
    private FurnitureFeaturesSo roomAttributes;
    
    [Space(50)]
    [Header("Debug")]
    [SerializeField]
    [Tooltip("Debug array of furniture features for testing")]
    private FurnitureFeaturesSo[] furnitureFeaturesSos;

    [FormerlySerializedAs("_lockedFurnitureBySize")] [Space(5), SerializedDictionary("Type", "Value"), SerializeField]
    private SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, int> lockedFurnitureBySize = new SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, int>();

    [Space(10)]
    [SerializeField] private CharacterValue characterHappiness;
    
    #endregion

    #region Private Fields


    /// <summary>Reference to the game configuration for language settings</summary>
    private ConfigurationSo _config;

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Initializes furniture dictionaries and populates debug furniture on awake
    /// </summary>
    private void Awake()
    {
        if (furnitureFeaturesSos.Length == 0) return;

        foreach (var furnitureFeature in furnitureFeaturesSos)
        {
            RoomCanvasStatic.Instance.AddUnlockedFurniture(furnitureFeature.Size, furnitureFeature);
        }

        foreach (var slot in Enum.GetValues(typeof(TypeOfEnvironmentCharacteristicEnum)))
        {
            lockedFurnitureBySize.Add((TypeOfEnvironmentCharacteristicEnum)slot, 0);
        }

    }

    /// <summary>
    /// Initializes the furniture list and activates slots based on room size
    /// </summary>
    private void OnEnable()
    {
        for (byte i = 0; i < slotFurnitureRooms.Length; i++)
        {
            listOfFurniture.Add(new Furniture());
        }

        for (byte i = 0; i < numberOfFurniture; i++)
        {
            slotFurnitureRooms[i].gameObject.SetActive(true);
        }

        InitializeConfiguration();

        AddPreferenceToCharacter();
    }

    /// <summary>
    /// Cleans up event subscriptions when the component is disabled
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Functionality

    private void AddPreferenceToCharacter()
    {
        if (character is null)
        {
            return;
        }

        List<FurnitureFeaturesSo> list = new List<FurnitureFeaturesSo> { roomAttributes };

        list.AddRange(furnitureFeaturesSos);

        float preference = character.CalculateAllPreferences(list);

        characterHappiness.character = character.Character;
        characterHappiness.value = preference;
    }

    #endregion

    #region Localization

    /// <summary>
    /// Loads configuration and subscribes to language change events
    /// </summary>
    private void InitializeConfiguration()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }

    /// <summary>
    /// Unsubscribes from language change events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    /// <summary>
    /// Updates UI text based on the selected language
    /// </summary>
    /// <param name="lang">Target language enum</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        // Language update logic to be implemented
    }

    #endregion

    #region Furniture Management

    /// <summary>
    /// Adds a new furniture piece to the first available empty slot
    /// </summary>
    /// <param name="newFurniture">The furniture features to add</param>
    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        foreach (var furniture in listOfFurniture)
        {
            if (furniture.furniture == null)
            {
                furniture.AddFurniture(newFurniture);
                float preference = characterHappiness.value;

                foreach (var characteristic in newFurniture.GetAllCharacteristics())
                {
                    lockedFurnitureBySize[characteristic.Key] += characteristic.Value.value;

                    if (character is null)
                    {
                        continue;
                    }
                    preference += character.CalculatePreference(characteristic.Key, lockedFurnitureBySize[characteristic.Key]);
                }

                if (character is null)
                {
                    break;
                }

                characterHappiness.character = character.Character;
                characterHappiness.value = preference;

                break;
            }
        }
    }

    /// <summary>
    /// Removes a specific furniture piece from the room
    /// </summary>
    /// <param name="furnitureToRemove">The furniture features to remove</param>
    public void RemoveFurniture(FurnitureFeaturesSo furnitureToRemove)
    {
        foreach (var furniture in listOfFurniture.Where(furniture => furniture.furniture == furnitureToRemove))
        {
            furniture.RemoveFurniture();
            float preference = characterHappiness.value;

            foreach (var characteristic in furnitureToRemove.GetAllCharacteristics())
            {
                int beginner = lockedFurnitureBySize[characteristic.Key];
                int newValue = beginner - characteristic.Value.value;
                lockedFurnitureBySize[characteristic.Key] = newValue;

                if (character is null)
                {
                    continue;
                }
                preference = character.CalculateRemove(characteristic.Key,beginner, newValue, preference);
            }

            if (character is null)
            {
                break;
            }

            characterHappiness.character = character.Character;
            characterHappiness.value = preference;

            break;
        }
    }

    /// <summary>
    /// Increases the room size by activating additional furniture slots
    /// </summary>
    /// <param name="addSizeRoom">Number of slots to add to the room</param>
    public void UpdateSizeRoom(byte addSizeRoom)
    {
        numberOfFurniture += addSizeRoom;

        for (byte i = 0; i < numberOfFurniture; i++)
        {
            slotFurnitureRooms[i].gameObject.SetActive(true);
        }
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the text component displaying the furniture name
    /// </summary>
    /// <returns>Reference to the name text component</returns>
    public TMP_Text GetNameFurniture() => nameFurniture;

    /// <summary>
    /// Gets the text component displaying furniture description
    /// </summary>
    /// <returns>Reference to the description text component</returns>
    public TMP_Text GetDescriptionFurniture() => descriptionFurniture;

    #endregion
}

/// <summary>
/// Represents a single furniture slot in the room
/// Handles furniture assignment and triggers events for furniture changes
/// </summary>
[Serializable]
public class Furniture
{
    /// <summary>Size category of the furniture in this slot</summary>
    public SizeOfFurnitureEnum sizeOfFurniture;

    /// <summary>Current furniture features assigned to this slot</summary>
    public FurnitureFeaturesSo furniture;

    /// <summary>Event triggered when furniture is added to this slot</summary>
    public event Action<FurnitureFeaturesSo> OnFurnitureAdded;

    /// <summary>Event triggered when furniture is removed from this slot</summary>
    public event Action<FurnitureFeaturesSo> OnFurnitureRemoved;

    /// <summary>
    /// Assigns new furniture to this slot and triggers the added event
    /// </summary>
    /// <param name="newFurniture">The furniture features to assign</param>
    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        furniture = newFurniture;
        sizeOfFurniture = newFurniture.Size;
        OnFurnitureAdded?.Invoke(furniture);
    }

    /// <summary>
    /// Removes furniture from this slot and triggers the removed event
    /// </summary>
    public void RemoveFurniture()
    {
        OnFurnitureRemoved?.Invoke(furniture);
        furniture = null;
    }
}


[Serializable]
public class CharacterValue
{
    public Character character;
    public float value;

    public CharacterValue(Character c, float v)
    {
        character = c;
        value = v;
    }
}