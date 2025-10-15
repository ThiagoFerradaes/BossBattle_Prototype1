using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Manages character interactions, furniture preferences, and friendship systems within the tavern.
/// </summary>
public class CharactersSystem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Character data and preferences configuration")] 
    private CharactersSo character;

    private readonly Dictionary<DialogueSystemSo, CharacterDialogue> _availableDialogues = new();
    private readonly Dictionary<FurnitureFeaturesSo, FurnitureFeatures> _furniture = new();
    private readonly Dictionary<TypeOfEnvironmentCharacteristic,int> _availableEvents = new();
    
    public event Action<DialogueSystemSo> OnDialogueEvent;

    [Tooltip("Local copy of the character's dialogue system")] 
    [SerializeField] private DialogueSystem dialogueSystem;
    
    [Tooltip("Current friendship level with the character")]
    public float friendship;

    private void Awake() => _ = InitializeCharacterSystem();
    
    /// <summary>
    /// Initializes the character system by setting up events, furniture, dialogues, and preferences.
    /// </summary>
    private async Task InitializeCharacterSystem()
    {
        try
        {
            _availableEvents.Clear();
            foreach(TypeOfEnvironmentCharacteristic characteristic in Enum.GetValues(typeof(TypeOfEnvironmentCharacteristic)))
            {
                _availableEvents[characteristic] = 0;
            }

            _furniture.Clear();
            _availableDialogues.Clear();

            
            if (character != null)
            {
                var dialogues = character.Dialogues();
                
                if (dialogues != null)
                    foreach (var dialogue in dialogues)
                        _availableDialogues[dialogue.Key] = dialogue.Value;
                
            }
            
            await LoadCharacter();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize character system: {e.Message}");
        }
    }

    #region Furniture Management

    /// <summary>
    /// Adds a single piece of furniture and updates friendship based on preferences.
    /// </summary>
    /// <param name="furnitureFeaturesSo">Furniture item to add</param>
    public async Task AddFurniture(FurnitureFeaturesSo furnitureFeaturesSo)
    {
        if (furnitureFeaturesSo == null) return;

        UpdateFurnitureCollection(furnitureFeaturesSo);
        UpdateFriendshipForFurniture(furnitureFeaturesSo);
        await UpdateFriendship();
        await SaveCharacter();
    }
    
    /// <summary>
    /// Adds multiple pieces of furniture and updates friendship accordingly.
    /// </summary>
    /// <param name="furnitureList">List of furniture items to add</param>
    public async Task AddFurnitureRange(List<FurnitureFeaturesSo> furnitureList)
    {
        if (furnitureList?.Any() != true) return;

        foreach (var furniture in furnitureList)
        {
            UpdateFurnitureCollection(furniture);
            UpdateFriendshipForFurniture(furniture);
        }
        await UpdateFriendship();
        await SaveCharacter();
    }
    
    /// <summary>
    /// Removes a single piece of furniture and updates friendship.
    /// </summary>
    /// <param name="furnitureFeaturesSo">Furniture item to remove</param>
    public async Task RemoveFurniture(FurnitureFeaturesSo furnitureFeaturesSo)
    {
        if (!_furniture.TryGetValue(furnitureFeaturesSo, out var value)) return;
        
        UpdateFriendshipForRemovedFurniture(value);
        if (value.RemoveOne())
            _furniture.Remove(furnitureFeaturesSo);
            
        await UpdateFriendship();
        await SaveCharacter();
    }
    
    /// <summary>
    /// Removes multiple pieces of furniture and updates friendship.
    /// </summary>
    /// <param name="furnitureList">List of furniture items to remove</param>
    public async Task RemoveFurnitureRange(List<FurnitureFeaturesSo> furnitureList)
    {
        if (furnitureList?.Any() != true) return;

        foreach (var furniture in furnitureList.Where(f => _furniture.ContainsKey(f)))
        {
            if (_furniture.TryGetValue(furniture, out var value))
            {
                UpdateFriendshipForRemovedFurniture(value);
                if (value.RemoveOne())
                    _furniture.Remove(furniture);
            }
        }
        await UpdateFriendship();
        await SaveCharacter();
    }

    /// <summary>
    /// Updates the furniture collection by adding or incrementing item count.
    /// </summary>
    /// <param name="furniture">Furniture item to update</param>
    private void UpdateFurnitureCollection(FurnitureFeaturesSo furniture)
    {
        if (_furniture.TryGetValue(furniture, out var existing))
        {
            existing.AddOne();
        }
        else
        {
            _furniture.Add(furniture, new FurnitureFeatures(furniture.GetAllCharacteristics()));
        }
    }

    /// <summary>
    /// Updates friendship based on added furniture characteristics.
    /// </summary>
    /// <param name="furniture">Added furniture item</param>
    private void UpdateFriendshipForFurniture(FurnitureFeaturesSo furniture)
    {
        foreach (var (key, value) in _furniture[furniture].Furniture)
        {
            _availableEvents[key] += value.value;
        }
    }

    /// <summary>
    /// Updates friendship based on removed furniture characteristics.
    /// </summary>
    /// <param name="value">Removed furniture features</param>
    private void UpdateFriendshipForRemovedFurniture(FurnitureFeatures value)
    {
        foreach (var (key, val) in value.Furniture)
        {
            _availableEvents[key] -= val.value;
        }
    }

    #endregion

    #region Friendship and Dialogue Management

    /// <summary>
    /// Updates overall friendship level based on current furniture preferences.
    /// </summary>
    private async Task UpdateFriendship()
    {
        friendship = _availableEvents.Sum(e => character.CalculatePreference(e.Key, e.Value));
        await UpdateFriendshipDialogues(friendship);
    }

    /// <summary>
    /// Updates available dialogues based on the current friendship level.
    /// </summary>
    /// <param name="newFriendship">Current friendship value</param>
    private Task UpdateFriendshipDialogues(float newFriendship)
    {
        var dialogueSystemSo = character.GetDialogueForFriendshipLevel(newFriendship, _availableDialogues);
        OnDialogueEvent?.Invoke(dialogueSystemSo);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Marks a dialogue as completed.
    /// </summary>
    /// <param name="endDialogue">Completed dialogue</param>
    private void EndDialogue(DialogueSystemSo endDialogue)
    {
        if(_availableDialogues.TryGetValue(endDialogue, out var dialogue))
            dialogue.hasOccurred = true;
    }

    private void OnEnable() => dialogueSystem.OnComplicitEvent += EndDialogue;

    private void OnDisable() => dialogueSystem.OnComplicitEvent -= EndDialogue;

    #endregion
    
    #region Save and Load System
    
    /// <summary>
    /// Loads character data from persistent storage.
    /// </summary>
    private static Task LoadCharacter()
    {
        // TODO: Implement character loading logic
        return Task.CompletedTask;
    }

    /// <summary>
    /// Saves current character data to persistent storage.
    /// </summary>
    private Task SaveCharacter()
    {
        // TODO: Implement character saving logic
        return Task.CompletedTask;
    }
    
    #endregion  
}

/// <summary>
/// Represents furniture characteristics and quantity tracking.
/// </summary>
[Serializable]
public class FurnitureFeatures
{
    public Dictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> Furniture { get; }
    public byte Count { get; private set; }
    
    /// <summary>
    /// Initializes a new instance of furniture features.
    /// </summary>
    /// <param name="characteristics">Initial characteristics dictionary</param>
    public FurnitureFeatures(IReadOnlyDictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> characteristics)
    {
        Furniture = characteristics.ToDictionary(x => x.Key, x => x.Value);
        Count = 1;
    }
    
    /// <summary>
    /// Increments the furniture count.
    /// </summary>
    public void AddOne() => Count++;

    /// <summary>
    /// Decrements the furniture count and indicates if an item should be removed.
    /// </summary>
    /// <returns>True if the count reaches zero</returns>
    public bool RemoveOne() => --Count <= 0;
}