using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// Scriptable Object that defines the properties and characteristics of furniture items
/// </summary>
[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeatures : ScriptableObject
{
    [Header("Furniture Cost")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    public SerializedDictionary<CostOfTheFurniture, byte> cost = new();
    
    [Space(20)]
    [Header("Furniture Characteristics")]
    [SerializedDictionary("Type", "Value"), SerializeField] 
    public SerializedDictionary<TypeOfEnvironmentCharacteristic, byte> characteristics = new();
    
    [Space(20)]
    [Header("Furniture Size")]
    public SizeOfFurniture size;
    
    [Space(20)]
    [Header("Furniture 3D Model")]
    public GameObject furniturePrefab;
    
    [Space(20)]
    [Header("Furniture 2D Sprite")]
    public Sprite furnitureSprite;
}

/// <summary>
/// Defines the available size categories for furniture items
/// </summary>
public enum SizeOfFurniture
{
    Small,
    Medium,
    Large
}