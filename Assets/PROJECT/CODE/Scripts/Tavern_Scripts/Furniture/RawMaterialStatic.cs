using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages the inventory of furniture materials/resources.
/// Tracks quantities of different material types used for furniture crafting and upgrades.
/// </summary>
public class RawMaterialStatic : MonoBehaviour
{
    #region Properties
    
    public static RawMaterialStatic Instance { get; private set; }
    
    public string slotSave;
    
    /// <summary>
    /// Dictionary containing the quantity of each furniture material type.
    /// Key: Material type enum
    /// Value: Quantity available
    /// </summary>
    public Dictionary<CostOfTheFurnitureEnum, uint> FurnitureQuantity { get; } = new();

    private string _savePath;
    
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion
    
    #region Public Methods

    public Task SetSlotSave(string nameSlot)
    {
        slotSave = nameSlot;
        
        _savePath = Path.Combine(Application.persistentDataPath, slotSave + "RawMaterial.json");
        
        return SaveInventory();
    }

    public string GetSlotSave()
    {
        return slotSave;
    }
    
    /// <summary>
    /// Adds a specified quantity of material types to the inventory.
    /// If the material doesn't exist in the inventory, it creates a new entry.
    /// </summary>
    /// <param name="materialType">The type of material to add</param>
    /// <param name="quantity">The amount to add (must be positive)</param>
    public void AddMaterialAmount(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        if (!FurnitureQuantity.ContainsKey(materialType))
        {
            CreateMaterial(materialType, quantity);
            return;
        }
        
        FurnitureQuantity[materialType] += quantity;
    }
    
    /// <summary>
    /// Removes a specified quantity of material types from the inventory.
    /// If the quantity to remove is greater than or equal to the available amount,
    /// the material entry is completely removed from the inventory.
    /// </summary>
    /// <param name="materialType">The type of material to remove</param>
    /// <param name="quantity">The amount to remove</param>
    public void RemoveMaterialAmount(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        if (!FurnitureQuantity.TryGetValue(materialType, out var currentQuantity))
        {
            return;
        }

        if (currentQuantity <= quantity)
        {
            DestroyMaterial(materialType);
            return;
        }
        
        FurnitureQuantity[materialType] -= quantity;
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Creates a new material entry in the inventory with the specified quantity.
    /// </summary>
    /// <param name="materialType">The type of material to create</param>
    /// <param name="quantity">The initial quantity</param>
    private void CreateMaterial(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        FurnitureQuantity.Add(materialType, quantity);
    }

    /// <summary>
    /// Removes a material entry completely from the inventory.
    /// </summary>
    /// <param name="materialType">The type of material to remove</param>
    private void DestroyMaterial(CostOfTheFurnitureEnum materialType)
    {
        FurnitureQuantity.Remove(materialType);
    }
    
    #endregion

    #region  Save and Load

    public async Task SaveInventory()
    {
        try
        {
            var inventorySaveByJson = new RawMaterialData(FurnitureQuantity);
            
            var json = JsonUtility.ToJson(inventorySaveByJson, false);
            await File.WriteAllTextAsync(_savePath, json);
            
            Debug.Log("Inventory save data saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving inventory configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    public async Task LoadInventoryByJson()
    {
        try
        {
            if (!File.Exists(_savePath))
            {
                Debug.LogWarning("Inventory save data not found.");
                return;
            }

            string json = await File.ReadAllTextAsync(_savePath);
            RawMaterialData loadedData = JsonUtility.FromJson<RawMaterialData>(json);

            if (loadedData?.furnitureList == null)
            {
                Debug.LogError("Invalid save data structure.");
                return;
            }
            FurnitureQuantity.Clear();
            FurnitureQuantity.AddRange(loadedData.ToDictionary());
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading inventory configuration: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    #endregion
}


#region SaveRoomData

/// <summary>
/// Serializable data structure for storing furniture configuration in a room
/// </summary>
[Serializable]
public class RawMaterialData
{
    public List<RawMaterialEntry> furnitureList;

    /// <summary>
    /// Creates a new FurnitureData instance from a dictionary
    /// </summary>
    /// <param name="furnitureDict">Dictionary mapping furniture slots to furniture</param>
    public RawMaterialData(Dictionary<CostOfTheFurnitureEnum, uint> furnitureDict)
    {
        furnitureList = new List<RawMaterialEntry>();
        
        foreach (var kvp in furnitureDict)
        {
            furnitureList.Add(new RawMaterialEntry(kvp.Key, kvp.Value));
        }
    }

    /// <summary>
    /// Converts the furniture list back to a dictionary
    /// </summary>
    /// <returns>Dictionary mapping furniture slots to furniture</returns>
    public Dictionary<CostOfTheFurnitureEnum, uint> ToDictionary()
    {
        var dict = new Dictionary<CostOfTheFurnitureEnum, uint>();
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
public class RawMaterialEntry
{
    public CostOfTheFurnitureEnum key;
    public uint furniture;

    /// <summary>
    /// Creates a new furniture entry
    /// </summary>
    /// <param name="k">Slot key</param>
    /// <param name="f">Furniture object</param>
    public RawMaterialEntry(CostOfTheFurnitureEnum k, uint f)
    {
        key = k;
        furniture = f;
    }
}

/// <summary>
/// Container class for serializing furniture save data to JSON
/// </summary>
[Serializable]
public class RawMaterialByJson
{
    public RawMaterialData saveFurniture;
}


#endregion
