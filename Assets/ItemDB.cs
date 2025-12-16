using MyEnum;
using UnityEngine;

public static class ItemDB
{
    private static ItemDatabase _db;
    
    private static ItemDatabase DB
    {
        get
        {
            if (_db == null)
                _db = Resources.Load<ItemDatabase>("ItemDatabase");

            return _db;
        }
    }

    public static Sprite GetIcon(SizeOfFurnitureEnum type)
    {
        return DB.GetImage(type);
    }

    public static TextBoxesSo GetName(SizeOfFurnitureEnum type)
    {
        return DB.GeString(type);
    }
    
    public static TextBoxesSo GetDescription(SizeOfFurnitureEnum type)
    {
        return DB.GeString(type);
    }
    
    public static Sprite GetIcon(RarityEnum type)
    {
        return DB.GetImage(type);
    }

    public static TextBoxesSo GetName(RarityEnum type)
    {
        return DB.GeString(type);
    }
    
    public static TextBoxesSo GetDescription(RarityEnum type)
    {
        return DB.GeString(type);
    }
    
    public static Sprite GetIcon(CostOfTheFurnitureEnum type)
    {
        return DB.GetImage(type);
    }

    public static TextBoxesSo GetName(CostOfTheFurnitureEnum type)
    {
        return DB.GeString(type);
    }
    
    public static TextBoxesSo GetDescription(CostOfTheFurnitureEnum type)
    {
        return DB.GeString(type);
    }
}