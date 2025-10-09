using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// Scriptable Object that defines the properties and characteristics of furniture items in the tavern.
/// This class manages the costs, characteristics, size, and visual representations of furniture.
/// </summary>
[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeatures : ScriptableObject
{
    #region Fields
    [Header("Furniture Cost")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    [Tooltip("Dictionary containing different types of costs and their values")]
    public SerializedDictionary<CostOfTheFurniture, byte> costs = new();
    
    [Space(20)]
    [Header("Furniture Characteristics")]
    [SerializedDictionary("Type", "Value"), SerializeField]
    [Tooltip("Dictionary containing environmental characteristics and their intensity values")]
    public SerializedDictionary<TypeOfEnvironmentCharacteristic, byte> characteristics = new();
    
    [Space(20)]
    [Header("Furniture Size")]
    [Tooltip("Defines the size category of the furniture")]
    public SizeOfFurniture size;
    
    [Space(20)]
    [Header("Furniture 3D Model")]
    [Tooltip("Reference to the 3D model prefab of the furniture")]
    public GameObject furniturePrefab;
    
    [Space(20)]
    [Header("Furniture 2D Sprite")]
    [Tooltip("2D sprite representation of the furniture for UI elements")]
    public Sprite furnitureSprite;
    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves the cost value for a specific cost type
    /// </summary>
    /// <param name="cost">The type of cost to query</param>
    /// <returns>The cost value as a byte</returns>
    public byte GetCost(CostOfTheFurniture cost)
    {
        return this.costs[cost];
    }
    
    /// <summary>
    /// Retrieves the characteristic value for a specific environmental characteristic
    /// </summary>
    /// <param name="characteristic">The type of characteristic to query</param>
    /// <returns>The characteristic value as a byte</returns>
    public byte GetCharacteristic(TypeOfEnvironmentCharacteristic characteristic)
    {
        return characteristics[characteristic];
    }

    /// <summary>
    /// Returns all costs associated with the furniture
    /// </summary>
    /// <returns>Dictionary containing all cost types and their values</returns>
    public Dictionary<CostOfTheFurniture, byte> GetAllCosts()
    {
        return costs;
    }
    
    /// <summary>
    /// Returns all characteristics associated with the furniture
    /// </summary>
    /// <returns>Dictionary containing all characteristic types and their values</returns>
    public Dictionary<TypeOfEnvironmentCharacteristic, byte> GetAllCharacteristics()
    {
        return characteristics;
    }
    #endregion
}