using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using MyEnum;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Manages the room canvas functionality including furniture unlocking, saving/loading, and UI prefab management
/// </summary>
public class RoomCanvasStatic : MonoBehaviour
{
    #region Variables
    public static RoomCanvasStatic Instance { get; private set; }
    
    [SerializeField]
    [Tooltip("Content Events and consequence for tutorial")]
    private List<Tutorial> tutorial;

    [SerializeField]
    [Tooltip("Object tutorial")]private GameObject tutorialOBj;
    
    private byte indexStage;

    private bool TutorialIsDone => indexStage >= tutorial.Count;

    private Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked = new();
    
    /// <summary>Dictionary mapping furniture features to their UI prefab instances</summary>
    private readonly Dictionary<FurnitureFeaturesSo, PrefabUiFurniture> prefabsFurniture = new();
    
    [Space(30)]
    [Header("features")]
    [SerializeField]
    [Tooltip("Content container where furniture UI prefabs will be instantiated")]
    private GameObject content;
    
    [SerializeField]
    [Tooltip("Array of room systems to manage")]
    private RoomSystem[] roomSystems;
    
    [SerializeField]
    [Tooltip("Prefab template for furniture UI list items")]
    private GameObject prefabFurniture;
    
    [SerializedDictionary("Spawner Type", "List of Spawners")]
    [SerializeField]
    [Tooltip("Dictionary mapping character spawner types to their corresponding spawner transform arrays")]
    private SerializedDictionary<SpawnerCharacterEnum, SpawnerNpc[]> spawners;
    
    [SerializedDictionary("Activity Type", "Spawner Mapping")]
    [SerializeField]
    [Tooltip("Dictionary mapping activity types to coded spawner configurations, where each activity links to room-specific spawner types")]
    private SerializedDictionary<ActivitiesEnum, SerializedDictionary<byte,SpawnerCharacterEnum>> codedSpawner;
    
    private RawMaterialStatic rawMaterialStatic;
    
    /// <summary>Path where furniture save data is stored</summary>
    private static string SavePath => Path.Combine(Application.persistentDataPath, RawMaterialStatic.Instance.GetSlotSave() + "FurnitureSave.json");
    
    /// <summary>Path where inventory save data is stored</summary>
    private static string SavePathForInventory => Path.Combine(Application.persistentDataPath, RawMaterialStatic.Instance.GetSlotSave() + "FurnitureSaveInventory.json");
    
    
    #endregion

    #region Unity Methods
    
    /// <summary>
    /// Initializes the singleton instance and loads saved data
    /// </summary>
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
            
            rawMaterialStatic = RawMaterialStatic.Instance;
            
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
    /// Saves the current furniture configuration to JSON file
    /// </summary>
    public async Task SaveFurnitureByJson()
    {
        try
        {
            if(!TutorialIsDone) return;
            
            // Extract data from room systems
            var furnitureData = roomSystems
                .Select(roomSystem => roomSystem.GetFurniture())
                .ToList();
            
            // Create a save data structure
            SaveFurniture saveFurnitureData = new SaveFurniture();

            foreach (var data in furnitureData)
            {
                byte roomKey = data.id;
                CharacterValue characterValue = data.characterHappiness;
                Dictionary<byte, Furniture> furnitureDict = data.furnitureDictionary;
                byte slotAmount = data.slotAmount;
                CharactersSo characterSo = data.characterSo;
                
                // Convert to serializable structure
                FurnitureData roomFurniture = new FurnitureData(characterValue, furnitureDict,characterSo.Character ,slotAmount);
                saveFurnitureData.furnitureRooms.Add(new RoomFurnitureEntry(roomKey, roomFurniture));
            }

            SaveFurnitureByJson saveFurniture = new SaveFurnitureByJson
            {
                saveFurniture = saveFurnitureData
            };

            // Serialize to JSON
            var json = JsonUtility.ToJson(saveFurniture, false);
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
    /// Loads furniture configuration from a JSON file
    /// </summary>
    private async Task LoadFurnitureByJson()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                //Debug.LogWarning("Furniture save data not found.");
                await LoadInventoryByJson();
                tutorialOBj.SetActive(true);
                NexStage();
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
            // Convert back to Dictionary
            Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>, byte, CharactersSo)> furnitureDictionary =
                new Dictionary<byte, (CharacterValue, Dictionary<byte, Furniture>, byte, CharactersSo)>();
            TimeEnum timeEnum = RawMaterialStatic.Instance.GetTimeGame();
            
            foreach (var roomEntry in loadedData.saveFurniture.furnitureRooms)
            {
                byte roomKey = roomEntry.roomKey;
                byte slotAmount = roomEntry.furnitureData.slotAmount;
                CharacterValue characterValue = roomEntry.furnitureData.characterValue;
                Dictionary<byte, Furniture> furnitureDict = roomEntry.furnitureData.ToDictionary();
                CharactersSo characterSo = ItemDB.GetCharacter(roomEntry.furnitureData.character);
                
                _ = InstantiateCharacter(characterSo,timeEnum, roomKey);
                furnitureDictionary[roomKey] = (characterValue, furnitureDict, slotAmount, characterSo);
            }
            // Apply loaded data to room systems
            int loadedCount = 0;
            foreach (var roomSystem in roomSystems)
            {
                byte id = roomSystem.ID();

                // Check if saved data exists for this room
                if (furnitureDictionary.TryGetValue(id, out var furnitureData))
                {
                    roomSystem.LoadFurniture(furnitureData.Item2, furnitureData.Item1, furnitureData.Item3,furnitureData.Item4);
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

    /// <summary>
    /// Saves the inventory data to JSON file
    /// </summary>
    private async Task SaveInventoryByJson()
    {
        try
        {
            var inventorySaveByJson = new InventorySaveByJson
            {
                saveInventory = new InventoryData(listOfFurnitureUnlocked)
            };
            
            var json = JsonUtility.ToJson(inventorySaveByJson, false);
            await File.WriteAllTextAsync(SavePathForInventory, json);
            
            Debug.Log("Inventory save data saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving inventory configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }

    /// <summary>
    /// Loads inventory data from a JSON file
    /// </summary>
    private async Task LoadInventoryByJson()
    {
        try
        {
            if (!File.Exists(SavePathForInventory))
            {
                //Debug.LogWarning("Inventory save data not found.");
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
            Debug.LogError($"Error loading inventory configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    #endregion

    #region private Methods

    private void NexStage(bool isStart = false)
    {
        //Debug.Log("tutorial stage completed");
        if (tutorial.Count == 0)
        {
            Debug.LogWarning("No tutorial stages found.");
            return;
        }

        if (isStart)
        {
            tutorial[indexStage].classForTutorial.OnCompleteTutorialEvent -= NexStage;
            tutorial[indexStage].unityEventForCompleteThisTutorial.Invoke();
            indexStage++;
        }

        if (indexStage >= tutorial.Count)
        {
            //Debug.Log("Tutorial completed.");
            return;
        }
        
        tutorial[indexStage].classForTutorial.OnCompleteTutorialEvent += NexStage;
    }
    
    /// <summary>
    /// Instantiates a character in the appropriate location based on their activity and time of day
    /// </summary>
    /// <param name="characterSo">Character data containing personality and activity preferences</param>
    /// <param name="timeEnum">Current time of day</param>
    /// <param name="roomKey">Room identifier for spawning</param>
    /// <returns>Task representing the asynchronous spawn operation</returns>
    private Task InstantiateCharacter(CharactersSo characterSo, TimeEnum timeEnum, byte roomKey)
    {
        if(characterSo is null) return Task.CompletedTask;
        
        ActivitiesEnum activity = characterSo.GetRandomActivity(timeEnum);
    
        var spawnerCharacterEnum = activity switch
        {
            ActivitiesEnum.Room => codedSpawner[activity][roomKey],
            _ => codedSpawner[activity][0],
        };
    
        var prefab = CanvasTavernaManagerStatic.Instance.GetCharacterPrefab(characterSo.Character);
        return prefab is null ? Task.CompletedTask : TrySpawn(activity, spawnerCharacterEnum, roomKey, prefab);
    }
    
    /// <summary>
    /// Attempts to spawn a character at the specified location with fallback logic.
    /// If spawning fails, try alternative locations in a predefined order.
    /// </summary>
    /// <param name="activity">Initial activity location to attempt spawn</param>
    /// <param name="spawnerCharacterEnum">Type of spawner to use</param>
    /// <param name="roomKey">Room identifier for fallback spawning</param>
    /// <param name="prefab">Character prefab to instantiate</param>
    /// <returns>Task representing the asynchronous spawn operation</returns>
    private async Task TrySpawn(ActivitiesEnum activity, SpawnerCharacterEnum spawnerCharacterEnum, byte roomKey, GameObject prefab)
    {
        const byte maxAttempts = 25;
        const byte maxFallbacks = 5;
    
        byte fallbackCount = 0;
    
        while (fallbackCount < maxFallbacks)
        {
            if (await TrySpawnInList(spawnerCharacterEnum, maxAttempts, prefab)) 
                return;
    
            // Fallback to alternative spawn locations in order:
            // Room -> CommonRoom -> Bathroom -> ArtifactRoom -> back to Room
            spawnerCharacterEnum = activity switch
            {
                ActivitiesEnum.Room => codedSpawner[ActivitiesEnum.CommonRoom][0],
                ActivitiesEnum.CommonRoom => codedSpawner[ActivitiesEnum.Bathroom][0],
                ActivitiesEnum.Bathroom => codedSpawner[ActivitiesEnum.ArtifactRoom][0],
                ActivitiesEnum.ArtifactRoom => codedSpawner[ActivitiesEnum.Room][roomKey],
                _ => codedSpawner[ActivitiesEnum.CommonRoom][0],
            };
    
            fallbackCount++;
        }
    
        Debug.LogWarning("No available spawners found after all fallback attempts.");
    }
    
    /// <summary>
    /// Attempts to spawn a character at a random unoccupied spawner from the specified list
    /// </summary>
    /// <param name="spawnerCharacterEnum">Type of spawner list to search</param>
    /// <param name="attempts">Maximum number of random spawner selections to try</param>
    /// <param name="prefab">Character prefab to instantiate</param>
    /// <returns>True if spawn was successful; false if all attempts failed</returns>
    private async Task<bool> TrySpawnInList(SpawnerCharacterEnum spawnerCharacterEnum, int attempts, GameObject prefab)
    {
        var list = spawners[spawnerCharacterEnum];
    
        for (var i = 0; i < attempts; i++)
        {
            var random = UnityEngine.Random.Range(0, list.Length);
    
            if (list[random].isSpawned) continue;
        
            list[random].isSpawned = true;
            await InstantiateAsync(prefab, list[random].spawner.position, list[random].spawner.rotation);
            return true;
        }
    
        return false;
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
    
    /// <summary>
    /// Gets the dictionary of furniture costs and quantities
    /// </summary>
    public Dictionary<CostOfTheFurnitureEnum, uint> GetCostFurnitureUnlocked => rawMaterialStatic.FurnitureQuantity;

    /// <summary>
    /// Adds raw material to the inventory
    /// </summary>
    /// <param name="cost">Type of raw material</param>
    /// <param name="amount">Quantity to add</param>
    public void AddRawMaterial(CostOfTheFurnitureEnum cost, uint amount)
    {
        rawMaterialStatic.AddMaterialAmount(cost, amount);
    }
    
    /// <summary>
    /// Removes raw material from the inventory
    /// </summary>
    /// <param name="cost">Type of raw material</param>
    /// <param name="amount">Quantity to remove</param>
    public void RemoveRawMaterial(CostOfTheFurnitureEnum cost, uint amount)
    {
        rawMaterialStatic.RemoveMaterialAmount(cost, amount);
    }
    
    
    #endregion

    #region Dictionary Management
    
    /// <summary>
    /// Adds a single furniture piece to the unlocked inventory.
    /// Increments count if furniture already exists.
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
    /// Removes one instance of a furniture piece from the unlocked inventory.
    /// Removes from the prefabs dictionary if the count reaches zero.
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
    /// Removes multiple furniture pieces from the unlocked inventory.
    /// Removes from prefab dictionary if any count reaches zero.
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
    /// Clears all unlocked furniture for a specific size category.
    /// Also clears all prefabs from the UI dictionary.
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

#region Class

[Serializable]
public class SpawnerNpc
{
    public Transform spawner;
    public bool isSpawned;
}

[Serializable]
public class Tutorial
{
    public TutorialClassBehaviour classForTutorial;
    
    public UnityEvent unityEventForCompleteThisTutorial;
}

#endregion

#region SaveRoomData

/// <summary>
/// Serializable data structure for storing furniture configuration in a room
/// </summary>
[Serializable]
public class FurnitureData
{
    public byte slotAmount;
    public Character character;
    public CharacterValue characterValue;
    public List<FurnitureEntry> furnitureList;

    /// <summary>
    /// Creates a new FurnitureData instance from a dictionary
    /// </summary>
    /// <param name="charValue">Character happiness value</param>
    /// <param name="furnitureDict">Dictionary mapping furniture slots to furniture</param>
    /// <param name="characterSo">ScriptObj For character</param>
    /// <param name="slotAmount">Number of furniture slots</param>
    public FurnitureData(CharacterValue charValue, Dictionary<byte, Furniture> furnitureDict,Character characterSo = Character.Null ,byte slotAmount = 0)
    {
        character = characterSo;
        characterValue = charValue;
        furnitureList = new List<FurnitureEntry>();
        this.slotAmount = slotAmount;
        
        foreach (var kvp in furnitureDict)
        {
            furnitureList.Add(new FurnitureEntry(kvp.Key, kvp.Value));
        }
    }

    /// <summary>
    /// Converts the furniture list back to a dictionary
    /// </summary>
    /// <returns>Dictionary mapping furniture slots to furniture</returns>
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
/// Serializable key-value pair for furniture entries
/// </summary>
[Serializable]
public class FurnitureEntry
{
    public byte key;
    public Furniture furniture;

    /// <summary>
    /// Creates a new furniture entry
    /// </summary>
    /// <param name="k">Slot key</param>
    /// <param name="f">Furniture object</param>
    public FurnitureEntry(byte k, Furniture f)
    {
        key = k;
        furniture = f;
    }
}

/// <summary>
/// Serializable key-value pair for room furniture data
/// </summary>
[Serializable]
public class RoomFurnitureEntry
{
    public byte roomKey;
    public FurnitureData furnitureData;

    /// <summary>
    /// Creates a new room furniture entry
    /// </summary>
    /// <param name="key">Room identifier</param>
    /// <param name="data">Furniture data for the room</param>
    public RoomFurnitureEntry(byte key, FurnitureData data)
    {
        roomKey = key;
        furnitureData = data;
    }
}

/// <summary>
/// Data structure for saving multiple rooms' furniture configuration
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

/// <summary>
/// Serializable data structure for storing inventory information
/// </summary>
[Serializable]
public class InventoryData
{
    public List<InventoryEntity> inventoryList = new();

    /// <summary>
    /// Creates a new InventoryData instance from the unlocked furniture dictionary
    /// </summary>
    /// <param name="listOfFurnitureUnlocked">Dictionary of unlocked furniture by size</param>
    public InventoryData(Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> listOfFurnitureUnlocked)
    {
        foreach (var kvp in listOfFurnitureUnlocked)
        {
            inventoryList.Add(new InventoryEntity(kvp.Key, kvp.Value));
        }
    }
    
    /// <summary>
    /// Converts the inventory list back to a dictionary
    /// </summary>
    /// <returns>Dictionary of unlocked furniture organized by size</returns>
    public Dictionary<SizeOfFurnitureEnum, Dictionary<FurnitureFeaturesSo, uint>> ToDictionary()
    {
        return inventoryList.ToDictionary(entry => entry.key, entry => entry.ToDictionary());
    }
}

/// <summary>
/// Serializable entity representing a furniture size category in the inventory
/// </summary>
[Serializable]
public class InventoryEntity
{
    public SizeOfFurnitureEnum key;
    public List<InventoryDataEntity> data = new();

    /// <summary>
    /// Creates a new inventory entity for a specific furniture size
    /// </summary>
    /// <param name="size">Furniture size category</param>
    /// <param name="dict">Dictionary of furniture and their quantities</param>
    public InventoryEntity(SizeOfFurnitureEnum size, Dictionary<FurnitureFeaturesSo, uint> dict)
    {
        key = size;
        foreach (var kvp in dict)
        {
            data.Add(new InventoryDataEntity {key = kvp.Key, value = kvp.Value});
        }
    }
    
    /// <summary>
    /// Converts the inventory data back to a dictionary
    /// </summary>
    /// <returns>Dictionary mapping furniture to quantities</returns>
    public Dictionary<FurnitureFeaturesSo, uint> ToDictionary()
    {
        return data.Where(entry => entry != null).Where(entry => entry.key != null).ToDictionary(entry => entry.key, entry => entry.value);
    }
}

/// <summary>
/// Serializable key-value pair for individual furniture items in inventory
/// </summary>
[Serializable]
public class InventoryDataEntity
{
    public FurnitureFeaturesSo key;
    public uint value;
}

/// <summary>
/// Container class for serializing inventory save data to JSON
/// </summary>
[Serializable]
public class InventorySaveByJson
{
    public InventoryData saveInventory;
}

#endregion

