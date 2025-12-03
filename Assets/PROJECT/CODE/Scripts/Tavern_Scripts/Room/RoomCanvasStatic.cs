using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Manages the room canvas functionality including furniture unlocking, saving/loading, and UI prefab management
/// </summary>
public class RoomCanvasStatic : MonoBehaviour
{
    #region Variables
    public static RoomCanvasStatic Instance { get; private set; }
        
    /// <summary>Dictionary storing unlocked furniture organized by size and features</summary>
    //private Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    private Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    /// <summary>Dictionary mapping furniture features to their UI prefab instances</summary>
    private readonly Dictionary<FurnitureFeaturesSo, PrefabUiFurniture> prefabsFurniture = new();
    
    [SerializeField]
    [Tooltip("Content container where furniture UI prefabs will be instantiated")]
    private GameObject content;
    
    [SerializeField]
    [Tooltip("Array of room systems to manage")]
    private RoomSystem[] roomSystems;
    
    [SerializeField]
    [Tooltip("Prefab template for furniture UI list items")]
    private GameObject prefabFurniture;
    
    [SerializeField]
    [Tooltip("Furniture inventory component")]
    private FurnitureInventory furnitureInventory;
    
    /// <summary>Path where furniture save data is stored</summary>
    private static string SavePath => Path.Combine(Application.persistentDataPath, "FurnitureSave.json");
    private static string SavePathForInventory => Path.Combine(Application.persistentDataPath, "FurnitureSaveInventory.json");
    
    
    #endregion

    #region Unity Methods

    private async void Awake()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        
            await LoadFurnitureByJson();
        
            foreach (SizeOfFurnitureEnum size in Enum.GetValues(typeof(SizeOfFurnitureEnum)))
            {
                listOfFurnitureUnlocked[size] = new Dictionary<FurnitureFeaturesSo, uint>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing RoomCanvasStatic: {e.Message}");
        }
    }
    
    #endregion

    #region Save/Load

    /// <summary>
    /// Saves the current furniture configuration to JSON
    /// </summary>
    public async Task SaveFurnitureByJson()
    {
        try
        {
            // Outer dado's dos room systems
            var furnitureData = roomSystems
                .Select(roomSystem => roomSystem.GetFurniture())
                .ToList();

            // creat save data
            SaveFurniture saveFurnitureData = new SaveFurniture();

            foreach (var data in furnitureData)
            {
                byte roomKey = data.id;
                CharacterValue characterValue = data.characterHappiness;
                Dictionary<byte, Furniture> furnitureDict = data.furnitureDictionary;
                byte slotAmount = data.slotAmount;
                
                // Converter para estrutura serializável
                FurnitureData roomFurniture = new FurnitureData(characterValue, furnitureDict, slotAmount);
                saveFurnitureData.furnitureRooms.Add(new RoomFurnitureEntry(roomKey, roomFurniture));
            }

            SaveFurnitureByJson saveFurniture = new SaveFurnitureByJson
            {
                saveFurniture = saveFurnitureData
            };

            // Serializar para JSON
            var json = JsonUtility.ToJson(saveFurniture, true); // true = formatado
            await File.WriteAllTextAsync(SavePath, json);

            Debug.Log("Furniture save data saved successfully!");

            await SaveInventoryByJson();
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            await SaveInventoryByJson();
        }
    }

    /// <summary>
    /// Loads furniture configuration from JSON 
    /// </summary>
    private async Task LoadFurnitureByJson()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("Furniture save data not found.");
                await LoadInventoryByJson();
                return;
            }

            string json = await File.ReadAllTextAsync(SavePath);
            SaveFurnitureByJson loadedData = JsonUtility.FromJson<SaveFurnitureByJson>(json);

            if (loadedData?.saveFurniture?.furnitureRooms == null)
            {
                Debug.LogError("Invalid save data structure.");
                await LoadInventoryByJson();
                return;
            }

            // Converter de volta para Dictionary
            Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>, byte)> furnitureDictionary =
                new Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>, byte)>();

            foreach (var roomEntry in loadedData.saveFurniture.furnitureRooms)
            {
                byte roomKey = roomEntry.roomKey;
                byte slotAmount = roomEntry.furnitureData.slotAmount;
                CharacterValue characterValue = roomEntry.furnitureData.characterValue;
                Dictionary<byte, Furniture> furnitureDict = roomEntry.furnitureData.ToDictionary();

                furnitureDictionary[roomKey] = (characterValue, furnitureDict, slotAmount);
            }

            // Aplicar dados carregados aos room systems
            int loadedCount = 0;
            foreach (var roomSystem in roomSystems)
            {
                byte id = roomSystem.ID();

                // Verificar se existe dados salvos para este room
                if (furnitureDictionary.TryGetValue(id, out var furnitureData))
                {
                    roomSystem.LoadFurniture(furnitureData.Item2, furnitureData.Item1, furnitureData.Item3);
                    loadedCount++;
                }
                else
                {
                    Debug.LogWarning($"No saved furniture data found for room ID: {id}");
                }
            }

            Debug.Log($"Furniture data loaded successfully! Loaded {loadedCount}/{roomSystems.Length} rooms.");
            await LoadInventoryByJson();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            await LoadInventoryByJson();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private async Task SaveInventoryByJson()
    {
        try
        {
            var inventorySaveByJson = new InventorySaveByJson
            {
                saveInventory = new InventoryData(listOfFurnitureUnlocked)
            };
            
            var json = JsonUtility.ToJson(inventorySaveByJson, true); // true = formatado
            await File.WriteAllTextAsync(SavePathForInventory, json);
            
            Debug.Log("inventary save data saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }

    private async Task LoadInventoryByJson()
    {
        try
        {
            if (!File.Exists(SavePathForInventory))
            {
                Debug.LogWarning("Furniture save data not found.");
                return;
            }

            string json = await File.ReadAllTextAsync(SavePathForInventory);
            InventorySaveByJson loadedData = JsonUtility.FromJson<InventorySaveByJson>(json);

            if (loadedData?.saveInventory.inventoryList == null)
            {
                Debug.LogError("Invalid save data structure.");
                return;
            }
            listOfFurnitureUnlocked = loadedData.saveInventory.ToDictionary();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    #endregion

    #region Properties

    /// <summary>
    /// Gets read-only access to the unlocked furniture list
    /// </summary>
    public IReadOnlyDictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> ListOfFurnitureUnlocked => listOfFurnitureUnlocked;

    /// <summary>
    /// Gets read-only access to furniture prefabs dictionary
    /// </summary> 
    public IReadOnlyDictionary<FurnitureFeaturesSo, PrefabUiFurniture> PrefabsFurniture => prefabsFurniture;

    /// <summary>
    /// Gets the content container GameObject
    /// </summary>
    public GameObject Content => content;

    /// <summary>
    /// Gets the furniture prefab template
    /// </summary>
    public GameObject PrefabFurniture => prefabFurniture;

    #endregion

    #region Public Methods
    
    /// <summary>
    /// Adds a furniture prefab to the UI prefabs dictionary
    /// </summary>
    /// <param name="furnitureFeaturesSo">The furniture features</param>
    /// <param name="prefabUiFurniture">The UI prefab instance</param>
    public void AddPrefabsFurniture(FurnitureFeaturesSo furnitureFeaturesSo, PrefabUiFurniture prefabUiFurniture) 
    {
        prefabsFurniture.Add(furnitureFeaturesSo, prefabUiFurniture);
    }
    
    public Dictionary<CostOfTheFurnitureEnum, uint> GetCostFurnitureUnlocked => furnitureInventory.FurnitureQuantity;

    public void AddRawMaterial(CostOfTheFurnitureEnum cost , uint amount)
    {
        furnitureInventory.AddAmountRawMaterial(cost, amount);
    }
    
    public void RemoveRawMaterial(CostOfTheFurnitureEnum cost , uint amount)
    {
        furnitureInventory.RemoveAmountRawMaterial(cost, amount);
    }
    
    
    #endregion

    #region Dictionary Management
    
    /// <summary>
    /// Adds a single furniture piece to the unlocked inventory
    /// Increments count if furniture already exists
    /// </summary>
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
    /// Removes from prefabs' dictionary if the count reaches zero
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
    /// Removes from prefab dictionary if any count reaches zero
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

#region SaveRoomData

/// <summary>
/// Wrapper for tuple replacement - serializable version
/// </summary>
[Serializable]
public class FurnitureData
{
    public byte slotAmount;
    public CharacterValue characterValue;
    public List<FurnitureEntry> furnitureList;

    public FurnitureData(CharacterValue charValue, Dictionary<byte, Furniture> furnitureDict, byte slotAmount = 0)
    {
        characterValue = charValue;
        furnitureList = new List<FurnitureEntry>();
        this.slotAmount = slotAmount;
        
        foreach (var kvp in furnitureDict)
        {
            furnitureList.Add(new FurnitureEntry(kvp.Key, kvp.Value));
        }
    }

    // Método para converter de volta para Dictionary
    public Dictionary<byte, Furniture> ToDictionary()
    {
        var dict = new Dictionary<byte, Furniture>();
        foreach (var entry in furnitureList)
        {
            dict[entry.key] = entry.furniture;
        }
        return dict;
    }
}

/// <summary>
/// Serializable key-value pair for furniture
/// </summary>
[Serializable]
public class FurnitureEntry
{
    public byte key;
    public Furniture furniture;

    public FurnitureEntry(byte k, Furniture f)
    {
        key = k;
        furniture = f;
    }
}

/// <summary>
/// Serializable key-value pair for room data
/// </summary>
[Serializable]
public class RoomFurnitureEntry
{
    public byte roomKey;
    public FurnitureData furnitureData;

    public RoomFurnitureEntry(byte key, FurnitureData data)
    {
        roomKey = key;
        furnitureData = data;
    }
}

/// <summary>
/// Data structure for saving furniture configuration
/// </summary>
[Serializable]
public class SaveFurniture
{
    public List<RoomFurnitureEntry> furnitureRooms = new();
}

/// <summary>
/// Container class for serializing furniture save data to JSON
/// </summary>
[Serializable]
public class SaveFurnitureByJson
{
    public SaveFurniture saveFurniture;
}


#endregion

#region SaveInventoryData

[Serializable]
public class InventoryData
{
    public List<InventoryEntity> inventoryList = new();

    public InventoryData(Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked)
    {
        foreach (var kvp in listOfFurnitureUnlocked)
        {
            inventoryList.Add(new InventoryEntity(kvp.Key, kvp.Value));
        }
    }
    
    public Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> ToDictionary()
    {
        var dict = new Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>>();
        foreach (var entry in inventoryList)
        {
            dict[entry.key] = entry.ToDictionary();
        }
        return dict;
    }
    
    private Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked;
}

[Serializable]
public class InventoryEntity
{
    public SizeOfFurnitureEnum key;
    public List<InventoryDataEntity> data = new();

    public InventoryEntity(SizeOfFurnitureEnum size, Dictionary<FurnitureFeaturesSo, uint> dict)
    {
        key = size;
        foreach (var kvp in dict)
        {
            data.Add(new InventoryDataEntity {key = kvp.Key, value = kvp.Value});
        }
    }
    
    public Dictionary<FurnitureFeaturesSo, uint> ToDictionary()
    {
        var dict = new Dictionary<FurnitureFeaturesSo, uint>();
        foreach (var entry in data)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
    
}

[Serializable]
public class InventoryDataEntity
{
    public FurnitureFeaturesSo key;
    public uint value;
}


[Serializable]
public class InventorySaveByJson
{
    public InventoryData saveInventory;
}

#endregion