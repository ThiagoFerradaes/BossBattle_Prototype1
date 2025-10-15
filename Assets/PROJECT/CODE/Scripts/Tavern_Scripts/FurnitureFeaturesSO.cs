using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines and manages furniture properties within the tavern system.
/// This class handles furniture costs, environmental effects, physical properties, and visual assets.
/// </summary>
[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeaturesSo : ScriptableObject
{
    #region Serialized Fields
    [Header("Furniture Cost")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    [Tooltip("Dictionary mapping cost types (e.g., gold, resources) to their respective values")]
    private SerializedDictionary<CostOfTheFurniture, ClampedVar<byte>> costs = new();
    
    [Space(20)]
    [Header("Environmental Impact")] 
    [SerializedDictionary("Type", "Value"), SerializeField]
    [Tooltip("Dictionary mapping environmental effects (e.g., comfort, atmosphere) to their impact values")]
    private SerializedDictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> characteristics = new();
    
    [Space(20)]
    [Header("Physical Properties")]
    [Tooltip("Specifies the furniture's size category (Small, Medium, Large)")]
    [SerializeField]
    private SizeOfFurniture size;

    [Space(20)]
    [Header("Visual Assets")]
    [Tooltip("Reference to the furniture's 3D model prefab")]
    [SerializeField]
    private GameObject furniturePrefab;
    
    [Space(20)]
    [SerializeField]
    [Tooltip("2D sprite representation for UI and inventory")]
    private Sprite furnitureSprite;
    
    [Space(20)]
    [Header("Texts")]
    [SerializeField]
    [Tooltip("Localized name of the furniture item")]
    private TextBoxesSo furnitureName;
    
    [Space(20)]
    [SerializeField]
    [Tooltip("Localized description of the furniture item")]
    private TextBoxesSo furnitureDescription;
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the size category of the furniture item.
    /// </summary>
    public SizeOfFurniture Size => size;

    /// <summary>
    /// Gets the localized name of the furniture item.
    /// </summary>
    public TextBoxesSo Name => furnitureName;
    
    /// <summary>
    /// Gets the localized description of the furniture item.
    /// </summary>
    public TextBoxesSo Description => furnitureDescription;
    
    /// <summary>
    /// Gets the furniture's 3D model prefab for instantiation.
    /// </summary>
    public GameObject Prefab => furniturePrefab;

    /// <summary>
    /// Gets the furniture's 2D sprite for UI representation.
    /// </summary>
    public Sprite Art => furnitureSprite;
    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves the cost value for the specific furniture cost type.
    /// </summary>
    /// <param name="costType">The type of cost to query.</param>
    /// <returns>The cost value (range: 0-255). Returns 0 if the cost type is not defined.</returns>
    public byte GetCost(CostOfTheFurniture costType) =>
        costs.TryGetValue(costType, out var cost) ? cost.value : (byte)0;
    
    /// <summary>
    /// Retrieves the intensity value for a specific environmental characteristic.
    /// </summary>
    /// <param name="type">The type of environmental effect to query.</param>
    /// <returns>The characteristic intensity (range: -128 to 127). Returns 0 if the characteristic is not defined.</returns>
    public sbyte GetCharacteristic(TypeOfEnvironmentCharacteristic type) =>
        characteristics.TryGetValue(type, out var characteristic) ? characteristic.value : (sbyte)0;

    /// <summary>
    /// Provides an immutable copy of all furniture costs.
    /// </summary>
    /// <returns>A read-only dictionary containing all cost types and their values.</returns>
    public IReadOnlyDictionary<CostOfTheFurniture, ClampedVar<byte>> GetAllCosts() => 
        new Dictionary<CostOfTheFurniture, ClampedVar<byte>>(costs);
    
    /// <summary>
    /// Provides an immutable copy of all environmental characteristics.
    /// </summary>
    /// <returns>A read-only dictionary containing all characteristic types and their values.</returns>
    public IReadOnlyDictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>> GetAllCharacteristics() => 
        new Dictionary<TypeOfEnvironmentCharacteristic, ClampedVar<sbyte>>(characteristics);
    #endregion
}

/// <summary>
/// Represents a value automatically clamped within the limits of its type.
/// Used for storing numerical values that must remain within their type's range.
/// </summary>
/// <typeparam name="T">The numeric type of the value (supported types: byte, sbyte)</typeparam>
[System.Serializable]
public class ClampedVar<T>
{
    [Tooltip("The current value, automatically constrained by type T's range")]
    public T value;
}