using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomSystem : MonoBehaviour
{
    [SerializeField] private List<Furniture> listOfFurniture;
    [SerializeField] private CharactersSo character;

    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        
    }
    
    
}

[Serializable]
public class Furniture
{
    public SizeOfFurniture sizeOfFurniture;
    public FurnitureFeaturesSo furniture;
    
    public event Action<FurnitureFeaturesSo> OnFurnitureAdded;
    public event Action<FurnitureFeaturesSo> OnFurnitureRemoved;

    public void AddFurniture(FurnitureFeaturesSo newFurniture)
    {
        furniture = newFurniture;
        OnFurnitureAdded?.Invoke(furniture);
    }
    
    public void RemoveFurniture()
    {
        OnFurnitureRemoved?.Invoke(furniture);
        furniture = null;
    }
}