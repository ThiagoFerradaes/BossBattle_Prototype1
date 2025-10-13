using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines and manages furniture properties within the tavern system.
/// Controls furniture costs, environmental characteristics, dimensions, and visual assets.
/// </summary>
[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeaturesSo : ScriptableObject
{
    #region Serialized Fields
    [Header("Furniture Cost")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    [Tooltip("Maps different cost types (e.g., gold, resources) to their numerical values")]
    private SerializedDictionary<CostOfTheFurniture, ClampedVar<byte>> costs = new();
    

    [Space(20)]
    [Header("Environmental Impact")] 
    [SerializedDictionary("Type", "Value"), SerializeField]
    [Tooltip("Maps environmental effects (e.g., comfort, atmosphere) to their intensity values")]
    private SerializedDictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> characteristics = new();
    
    [Space(20)]
    [Header("Physical Properties")]
    [Tooltip("Defines the furniture's size category (Small, Medium, Large)")]
    [SerializeField]
    private SizeOfFurniture size;
    
    

    [Space(20)]
    [Header("Visual Assets")]
    [Tooltip("3D model prefab for gameplay visualization")]
    [SerializeField]
    private GameObject furniturePrefab;
    
    [SerializeField]
    [Tooltip("2D sprite for UI elements and inventory display")]
    private Sprite furnitureSprite;
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the size category of the furniture item
    /// </summary>
    public SizeOfFurniture Size => size;

    /// <summary>
    /// Gets the furniture's 3D model prefab
    /// </summary>
    public GameObject Prefab => furniturePrefab;

    /// <summary>
    /// Gets the furniture's 2D sprite representation
    /// </summary>
    public Sprite Art => furnitureSprite;
    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves the cost value for the specific furniture cost type
    /// </summary>
    /// <param name="costType">The type of cost to query</param>
    /// <returns>The cost value (0-255)</returns>
    public byte GetCost(CostOfTheFurniture costType) =>
        costs.TryGetValue(costType, out var cost) ? cost.value : (byte)0;
    
    /// <summary>
    /// Retrieves the intensity value for a specific environmental characteristic
    /// </summary>
    /// <param name="type">The type of environmental effect to query</param>
    /// <returns>The characteristic intensity (-128 to 127)</returns>
    public sbyte GetCharacteristic(TypeOfEnvironmentCharacteristic type) =>
        characteristics.TryGetValue(type, out var characteristic) ? characteristic.value : (sbyte)0;

    /// <summary>
    /// Gets an immutable copy of all furniture costs
    /// </summary>
    /// <returns>Read-only dictionary of cost types and their values</returns>
    public IReadOnlyDictionary<CostOfTheFurniture, ClampedVar<byte>> GetAllCosts() => 
        new Dictionary<CostOfTheFurniture, ClampedVar<byte>>(costs);
    
    /// <summary>
    /// Gets an immutable copy of all environmental characteristics
    /// </summary>
    /// <returns>Read-only dictionary of characteristic types and their values</returns>
    public IReadOnlyDictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> GetAllCharacteristics() => 
        new Dictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>>(characteristics);
    #endregion
}

/// <summary>
/// Represents a value automatically clamped to the limits of its type
/// </summary>
/// <typeparam name="T">The numeric type of the value (byte or sbyte)</typeparam>
[System.Serializable]
public class ClampedVar<T>
{
    [Tooltip("Current value, automatically limited by type T's range")]
    public T value;
}