using System;
using System.Collections.Generic;
using System.Linq;
using MyEnum;
using TMPro;
using UnityEngine;

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
    
    [Space(50)]
    [Header("Debug")]
    [SerializeField]
    [Tooltip("Debug array of furniture features for testing")]
    private FurnitureFeaturesSo[] furnitureFeaturesSos;
    
    #endregion
    
    #region Public Fields
    
    /// <summary>Dictionary storing unlocked furniture organized by size and features</summary>
    public readonly Dictionary<SizeOfFurniture, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    /// <summary>Dictionary mapping furniture features to their UI prefab instances</summary>
    public Dictionary<FurnitureFeaturesSo, PrefabUiFurniture> prefabsFurniture = new();
    
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
        foreach (SizeOfFurniture size in Enum.GetValues(typeof(SizeOfFurniture)))
        {
            listOfFurnitureUnlocked[size] = new Dictionary<FurnitureFeaturesSo, uint>();
        }

        foreach (var furnitureFeature in furnitureFeaturesSos)
        {
            AddUnlockedFurniture(furnitureFeature.Size, furnitureFeature);
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
    }

    /// <summary>
    /// Cleans up event subscriptions when the component is disabled
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
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
        foreach (var furniture in listOfFurniture.Where(furniture => furniture is null))
        {
            furniture?.AddFurniture(newFurniture);
            break;
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

    #region Dictionary Management
    
    /// <summary>
    /// Adds a single furniture piece to the unlocked inventory
    /// Increments count if furniture already exists
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="furniture">The furniture features to unlock</param>
    public void AddUnlockedFurniture(SizeOfFurniture sizeOfFurniture, FurnitureFeaturesSo furniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }

        if (value.TryAdd(furniture, 1)) return;
        
        value[furniture]++;
    }

    /// <summary>
    /// Replaces the entire furniture list for a specific size category
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="listFurniture">Dictionary of furniture and their quantities</param>
    public void AddListFurniture(SizeOfFurniture sizeOfFurniture, IReadOnlyDictionary<FurnitureFeaturesSo, uint> listFurniture)
    {
        if (!listOfFurnitureUnlocked.ContainsKey(sizeOfFurniture))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }

        listOfFurnitureUnlocked[sizeOfFurniture] = listFurniture.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    /// <summary>
    /// Removes one instance of a furniture piece from the unlocked inventory
    /// Removes from prefabs dictionary if count reaches zero
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="furniture">The furniture features to remove</param>
    public void RemoveUnlockedFurniture(SizeOfFurniture sizeOfFurniture, FurnitureFeaturesSo furniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }
        
        if (!value.TryGetValue(furniture, out var count)) return;

        count--;
        if (count > 0) return;
        
        value.Remove(furniture);
        prefabsFurniture.Remove(furniture);
    }

    /// <summary>
    /// Removes multiple furniture pieces from the unlocked inventory
    /// Removes from prefabs dictionary if any count reaches zero
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="listFurniture">Dictionary of furniture and quantities to remove</param>
    public void RemoveListFurniture(SizeOfFurniture sizeOfFurniture,
        IReadOnlyDictionary<FurnitureFeaturesSo, uint> listFurniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }
        
        foreach (var furniture in listFurniture)
        {
            value[furniture.Key] -= furniture.Value;
            if (value[furniture.Key] > 0) continue;
            
            value.Remove(furniture.Key);
            prefabsFurniture.Remove(furniture.Key);
        }
    }

    /// <summary>
    /// Clears all unlocked furniture for a specific size category
    /// Also clears all prefabs from the UI dictionary
    /// </summary>
    /// <param name="sizeOfFurniture">Size category to clear</param>
    public void ClearDictionary(SizeOfFurniture sizeOfFurniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }
        value.Clear();
        prefabsFurniture.Clear();
    }

    /// <summary>
    /// Removes all instances of a specific furniture type from unlocked inventory
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="furniture">The furniture features to remove completely</param>
    public void RemoveAllFurnitureByType(SizeOfFurniture sizeOfFurniture, FurnitureFeaturesSo furniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }
        value.Remove(furniture);
        prefabsFurniture.Remove(furniture);
    }

    /// <summary>
    /// Removes all instances of multiple furniture types from unlocked inventory
    /// </summary>
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="furniture">Array of furniture features to remove completely</param>
    public void RemoveAllFurnitureByList(SizeOfFurniture sizeOfFurniture, FurnitureFeaturesSo[] furniture)
    {
        if (!listOfFurnitureUnlocked.TryGetValue(sizeOfFurniture, out var value))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }
        
        foreach (var furnitureToRemove in furniture)
        {
            value.Remove(furnitureToRemove);
            prefabsFurniture.Remove(furnitureToRemove);
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
    public SizeOfFurniture sizeOfFurniture;
    
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