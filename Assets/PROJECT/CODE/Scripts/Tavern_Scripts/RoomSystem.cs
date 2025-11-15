using System;
using System.Collections.Generic;
using System.Linq;
using MyEnum;
using TMPro;
using UnityEngine;

public class RoomSystem : MonoBehaviour
{
    [SerializeField] private List<Furniture> listOfFurniture = new List<Furniture>();
    [SerializeField] private CharactersSo character;
    [SerializeField] private SlotFurnitureRoom[] slotFurnitureRooms;
    [SerializeField]private byte numberOfFurniture;

    [SerializeField] private TMP_Text nameFurniture , descriptionFurniture;
    
    public readonly Dictionary<SizeOfFurniture, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    public Dictionary<FurnitureFeaturesSo, PrefebUiFurniture> prefabsFurniture = new();
    
    private ConfigurationSo _config;
    
    
    #region Unity Lifecycle Methods
    
    private void Awake()
    {
        foreach (SizeOfFurniture size in Enum.GetValues(typeof(SizeOfFurniture)))
        {
            listOfFurnitureUnlocked[size] = new Dictionary<FurnitureFeaturesSo, uint>();
        }
    }
    
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

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    #endregion
    
    #region Lang
    
    private void InitializeConfiguration()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }
    
    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }
    
    private void UpdateLanguage(EnumLanguage lang)
    {

    }
    
    #endregion
    
    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        foreach (var furniture in listOfFurniture.Where(furniture => furniture is null))
        {
            furniture?.AddFurniture(newFurniture);
            break;
        }
    }

    public void RemoveFurniture(FurnitureFeaturesSo furnitureToRemove)
    {
        foreach (var furniture in listOfFurniture.Where(furniture => furniture.furniture == furnitureToRemove))
        {
            furniture.RemoveFurniture();
            break;
        }
    }

    public void UpdateSizeRoom(byte addSizeRoom)
    {
        numberOfFurniture += addSizeRoom;

        for (byte i = 0; i < numberOfFurniture; i++)
        {
            slotFurnitureRooms[i].gameObject.SetActive(true);
        }
    }

    #region Dictionary
    
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

    public void AddListFurniture(SizeOfFurniture sizeOfFurniture, IReadOnlyDictionary<FurnitureFeaturesSo, uint> listFurniture)
    {
        if (!listOfFurnitureUnlocked.ContainsKey(sizeOfFurniture))
        {
            Debug.LogWarning($"Unlocked furniture for size {sizeOfFurniture} not found.");
            return;
        }

        listOfFurnitureUnlocked[sizeOfFurniture] = listFurniture.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

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
    
    public TMP_Text GetNameFurniture() => nameFurniture;
    public TMP_Text GetDescriptionFurniture() => descriptionFurniture;
}

[Serializable]
public class Furniture
{
    public SizeOfFurniture sizeOfFurniture;
    public FurnitureFeaturesSo furniture;
    
    public event Action<FurnitureFeaturesSo> OnFurnitureAdded;
    public event Action<FurnitureFeaturesSo> OnFurnitureRemoved;

    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        furniture = newFurniture;
        OnFurnitureAdded?.Invoke(furniture);
    }
    
    public void RemoveFurniture()
    {
        OnFurnitureRemoved?.Invoke(furniture);
        furniture = null;
    }
}