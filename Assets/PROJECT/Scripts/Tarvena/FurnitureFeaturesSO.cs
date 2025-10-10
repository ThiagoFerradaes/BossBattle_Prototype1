using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines and manages furniture properties within the tavern system.
/// This class handles furniture costs, environmental characteristics, dimensions, and visual assets.
/// </summary>
[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeaturesSo : ScriptableObject
{
    #region Serialized Fields
    [Header("Furniture Cost")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    [Tooltip("Dictionary mapping cost types to their corresponding numerical values")]
    private SerializedDictionary<CostOfTheFurniture, byte> costs = new();
    
    [Space(20)]
    [Header("Furniture Characteristics")] 
    [SerializedDictionary("Type", "Value"), SerializeField]
    [Tooltip("Dictionary mapping environmental effects to their intensity values")]
    private SerializedDictionary<TypeOfEnvironmentCharacteristic, byte> characteristics = new();
    
    [Space(20)]
    [Header("Physical Properties")]
    [Tooltip("Size category of the furniture item")]
    [SerializeField]
    private SizeOfFurniture size;
    
    [Space(20)]
    [Header("Visual Assets")]
    [Tooltip("3D model prefab for in-game representation")]
    [SerializeField]
    private GameObject furniturePrefab;
    
    [SerializeField]
    [Tooltip("2D sprite used for UI and inventory representation")]
    private Sprite furnitureSprite;
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets or sets the size category of the furniture
    /// </summary>
    public SizeOfFurniture Size => size;

    /// <summary>
    /// Gets or sets the furniture's 3D model prefab representation
    /// </summary>
    public GameObject Prefab  => furniturePrefab;

    /// <summary>
    /// Gets or sets the furniture's 2D sprite for UI display
    /// </summary>
    public Sprite Art  => furnitureSprite;

    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves the cost value for a specified furniture cost type
    /// </summary>
    /// <param name="costType">The type of cost to query</param>
    /// <returns>The cost value as an unsigned byte</returns>
    public byte GetCost(CostOfTheFurniture costType) =>
        costs.GetValueOrDefault(costType, (byte)0);
    
    /// <summary>
    /// Retrieves the intensity value for a specified environmental characteristic
    /// </summary>
    /// <param name="type">The type of environmental effect to query</param>
    /// <returns>The characteristic intensity as an unsigned byte</returns>
    public byte GetCharacteristic(TypeOfEnvironmentCharacteristic type) =>
        characteristics.GetValueOrDefault(type, (byte)0);

    /// <summary>
    /// Gets all furniture costs as an immutable dictionary
    /// </summary>
    /// <returns>Read-only dictionary of cost types and their values</returns>
    public IReadOnlyDictionary<CostOfTheFurniture, byte> GetAllCosts() => 
        new Dictionary<CostOfTheFurniture, byte>(costs);
    
    /// <summary>
    /// Gets all environmental characteristics as an immutable dictionary
    /// </summary>
    /// <returns>Read-only dictionary of characteristic types and their intensity values</returns>
    public IReadOnlyDictionary<TypeOfEnvironmentCharacteristic, byte> GetAllCharacteristics() => 
        new Dictionary<TypeOfEnvironmentCharacteristic, byte>(characteristics);
    
    #endregion
}