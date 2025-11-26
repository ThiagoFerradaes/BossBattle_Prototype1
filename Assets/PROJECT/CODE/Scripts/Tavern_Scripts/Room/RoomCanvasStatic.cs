using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Manages the room canvas functionality including furniture unlocking, saving/loading and UI prefab management
/// </summary>
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
    [Tooltip("Array of room systems to manage")]
    private RoomSystem[] roomSystems;
    
    [SerializeField]
    [Tooltip("Prefab template for furniture UI list items")]
    private GameObject prefabFurniture;
    
    /// <summary>Path where furniture save data is stored</summary>
    private static string SavePath => Path.Combine(Application.persistentDataPath, "FurnitureSave.json");
    
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
    public Task SaveFurnitureByJson()
    {
        try
        {
            // Obter dados dos room systems
            var furnitureData = roomSystems
                .Select(roomSystem => roomSystem.GetFurniture())
                .ToList();

            // Criar estrutura serializável
            SaveFurniture saveFurnitureData = new SaveFurniture();

            foreach (var data in furnitureData)
            {
                byte roomKey = data.Item2;
                CharacterValue characterValue = data.Item3;
                Dictionary<byte, Furniture> furnitureDict = data.Item1;

                // Converter para estrutura serializável
                FurnitureData roomFurniture = new FurnitureData(characterValue, furnitureDict);
                saveFurnitureData.furnitureRooms.Add(new RoomFurnitureEntry(roomKey, roomFurniture));
            }

            SaveFurnitureByJson saveFurniture = new SaveFurnitureByJson
            {
                saveFurniture = saveFurnitureData
            };

            // Serializar para JSON
            var json = JsonUtility.ToJson(saveFurniture, true); // true = formatado
            File.WriteAllText(SavePath, json);

            Debug.Log("Furniture save data saved successfully!");
            Debug.Log(json);

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Loads furniture configuration from JSON 
    /// </summary>
    private Task LoadFurnitureByJson()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("Furniture save data not found.");
                return Task.CompletedTask;
            }

            string json = File.ReadAllText(SavePath);
            SaveFurnitureByJson loadedData = JsonUtility.FromJson<SaveFurnitureByJson>(json);

            if (loadedData?.saveFurniture?.furnitureRooms == null)
            {
                Debug.LogError("Invalid save data structure.");
                return Task.CompletedTask;
            }

            // Converter de volta para Dictionary
            Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>)> furnitureDictionary =
                new Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>)>();

            foreach (var roomEntry in loadedData.saveFurniture.furnitureRooms)
            {
                byte roomKey = roomEntry.roomKey;
                CharacterValue characterValue = roomEntry.furnitureData.characterValue;
                Dictionary<byte, Furniture> furnitureDict = roomEntry.furnitureData.ToDictionary();

                furnitureDictionary[roomKey] = (characterValue, furnitureDict);
            }

            // Aplicar dados carregados aos room systems
            int loadedCount = 0;
            foreach (var roomSystem in roomSystems)
            {
                byte id = roomSystem.ID();

                // Verificar se existe dados salvos para este room
                if (furnitureDictionary.TryGetValue(id, out var furnitureData))
                {
                    roomSystem.LoadFurniture(furnitureData.Item2, furnitureData.Item1);
                    loadedCount++;
                }
                else
                {
                    Debug.LogWarning($"No saved furniture data found for room ID: {id}");
                }
            }

            Debug.Log($"Furniture data loaded successfully! Loaded {loadedCount}/{roomSystems.Length} rooms.");
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading furniture configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
            return Task.CompletedTask;
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

/// <summary>
/// Wrapper for tuple replacement - serializable version
/// </summary>
[Serializable]
public class FurnitureData
{
    public CharacterValue characterValue;
    public List<FurnitureEntry> furnitureList;

    public FurnitureData(CharacterValue charValue, Dictionary<byte, Furniture> furnitureDict)
    {
        characterValue = charValue;
        furnitureList = new List<FurnitureEntry>();
        
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