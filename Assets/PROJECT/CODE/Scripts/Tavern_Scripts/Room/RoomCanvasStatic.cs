using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class RoomCanvasStatic : MonoBehaviour
{
    #region Variables
    public static RoomCanvasStatic Instance { get; private set; }
        
    /// <summary>Dictionary storing unlocked furniture organized by size and features</summary>
    private readonly Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    /// <summary>Dictionary mapping furniture features to their UI prefab instances</summary>
    private readonly Dictionary<FurnitureFeaturesSo, PrefabUiFurniture> prefabsFurniture = new();
    
    [SerializeField]
    [Tooltip("Content container where furniture UI prefabs will be instantiated")]
    private GameObject content;
    
    [SerializeField]
    [Tooltip("room System")]
    private RoomSystem[]  roomSystems;
    
    [SerializeField]
    [Tooltip("Prefab template for furniture UI list items")]
    private GameObject prefabFurniture;
    
    private static string SavePath => Path.Combine(Application.persistentDataPath, "FurnitureSave.json");
    
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Load Save
        LoadFurnitureByJson();
        
        // Initialize the furniture dictionary for each size category
        foreach (SizeOfFurnitureEnum size in Enum.GetValues(typeof(SizeOfFurnitureEnum)))
        {
            listOfFurnitureUnlocked[size] = new Dictionary<FurnitureFeaturesSo, uint>();
        }
    }

    private void OnDestroy()
    {
        SaveFurnitureByJson();
    }
    
    #endregion

    #region Save

    private void SaveFurnitureByJson()
    {
        try
        {
            Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>)> furnitureDictionary2 =
                roomSystems.Select(roomSystem => roomSystem.GetFurniture())
                    .ToDictionary(a => a.Item2, a => (a.Item3, a.Item1));

            SaveFurnitureByJson saveFurniture = new SaveFurnitureByJson
            {
                saveFurniture = new SaveFurniture
                {
                    furniture = furnitureDictionary2
                }
            };

            var json = JsonUtility.ToJson(saveFurniture, false);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving configuration: {e.Message}");
        }
    }

    private void LoadFurnitureByJson()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveFurnitureByJson>(json);

            foreach (var roomSystem in roomSystems)
            {
                byte id = roomSystem.ID();
                roomSystem.LoadFurniture(data.saveFurniture.furniture[id].Item2, data.saveFurniture.furniture[id].Item1);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading configuration: {e.Message}");
        }
    }

    #endregion

    #region Get Var

    public IReadOnlyDictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> ListOfFurnitureUnlocked => listOfFurnitureUnlocked;

    public IReadOnlyDictionary<FurnitureFeaturesSo, PrefabUiFurniture> PrefabsFurniture => prefabsFurniture;

    public GameObject Content => content;

    public GameObject PrefabFurniture => prefabFurniture;

    #endregion

    #region Set Var
    
    public void AddPrefabsFurniture(FurnitureFeaturesSo furnitureFeaturesSo, PrefabUiFurniture prefabUiFurniture) 
    {
        prefabsFurniture.Add(furnitureFeaturesSo, prefabUiFurniture);
    }

    #endregion

    #region Dictionary Management
    
    /// <summary>
    /// Adds a single furniture piece to the unlocked inventory
    /// Increments count if furniture already exists
    /// </summary>        Debug.Log("Furniture slot clicked");
    /// <param name="sizeOfFurniture">Size category of the furniture</param>
    /// <param name="furniture">The furniture features to unlock</param>
    public void AddUnlockedFurniture(SizeOfFurnitureEnum sizeOfFurniture, FurnitureFeaturesSo furniture)
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
    public void AddListFurniture(SizeOfFurnitureEnum sizeOfFurniture, IReadOnlyDictionary<FurnitureFeaturesSo, uint> listFurniture)
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
    public void RemoveUnlockedFurniture(SizeOfFurnitureEnum sizeOfFurniture, FurnitureFeaturesSo furniture)
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
    public void RemoveListFurniture(SizeOfFurnitureEnum sizeOfFurniture,
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
    public void ClearDictionary(SizeOfFurnitureEnum sizeOfFurniture)
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
    public void RemoveAllFurnitureByType(SizeOfFurnitureEnum sizeOfFurniture, FurnitureFeaturesSo furniture)
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
    public void RemoveAllFurnitureByList(SizeOfFurnitureEnum sizeOfFurniture, FurnitureFeaturesSo[] furniture)
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
}

[Serializable]
public class SaveFurniture
{
    public Dictionary<byte, (CharacterValue,Dictionary<byte, Furniture>)> furniture;
}

[Serializable]
public class SaveFurnitureByJson
{
    public SaveFurniture saveFurniture;
}