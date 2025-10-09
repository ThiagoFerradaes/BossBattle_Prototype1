using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FurnitureFeatures", menuName = "Scriptable Objects/FurnitureFeatures")]
public class FurnitureFeatures : ScriptableObject
{
    [Header("cost of the furniture")]
    [SerializedDictionary("Type", "Valor"), SerializeField] 
    public SerializedDictionary<CostOfTheFurniture, byte> cost  = new();
    
    [Space(20)]
    [Header("Furniture Features")]
    [SerializedDictionary("Type", "Valor"), SerializeField] 
    public SerializedDictionary<TypeOfEnvironmentCharacteristic, byte> furniture = new();
    
    [Space(20)]
    [Header("Size of Furniture")]
    public SizeOfFurniture size;
    
    [Space(20)]
    [Header("Prefab Of Furniture")]
    public GameObject furniturePrefab;
    
    [Space(20)]
    [Header("2D art of Furniture")]
    public Sprite furnitureSprite;
}

public enum SizeOfFurniture
{
    Small, 
    Medium, 
    Large
}