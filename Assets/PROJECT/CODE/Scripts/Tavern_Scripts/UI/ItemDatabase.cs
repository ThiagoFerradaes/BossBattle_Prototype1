using UnityEngine;
using AYellowpaper.SerializedCollections;
using MyEnum;


[CreateAssetMenu(
    fileName = "ItemDatabase",
    menuName = "Database/Item Database"
)]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemInfoForSize
    {
        public Sprite icon;
        public TextBoxesSo displayName;
    }

    [SerializedDictionary("Size", "Info")]
    [SerializeField] private SerializedDictionary<SizeOfFurnitureEnum, ItemInfoForSize> itemsBySize = new();
    
    [SerializedDictionary("rarity", "Info")]
    [SerializeField] private SerializedDictionary<RarityEnum, ItemInfoForSize> itemsByRarity = new();
    
    [SerializedDictionary("cost", "Info")]
    [SerializeField] private SerializedDictionary<CostOfTheFurnitureEnum, ItemInfoForSize> itemsByCost = new();
    public Sprite GetImage(SizeOfFurnitureEnum type)
    {
        return !itemsBySize.ContainsKey(type) ? null : itemsBySize[type].icon;
    }
    
    public TextBoxesSo GeString(SizeOfFurnitureEnum type)
    {
        return !itemsBySize.ContainsKey(type) ? null : itemsBySize[type].displayName;
    }
    
    public Sprite GetImage(RarityEnum type)
    {
        return !itemsByRarity.ContainsKey(type) ? null : itemsByRarity[type].icon;
    }
    
    public TextBoxesSo GeString(RarityEnum type)
    {
        return !itemsByRarity.ContainsKey(type) ? null : itemsByRarity[type].displayName;
    }
    
    public Sprite GetImage(CostOfTheFurnitureEnum type)
    {
        return !itemsByCost.ContainsKey(type) ? null : itemsByCost[type].icon;
    }
    
    public TextBoxesSo GeString(CostOfTheFurnitureEnum type)
    {
        return !itemsByCost.ContainsKey(type) ? null : itemsByCost[type].displayName;
    }
    
}