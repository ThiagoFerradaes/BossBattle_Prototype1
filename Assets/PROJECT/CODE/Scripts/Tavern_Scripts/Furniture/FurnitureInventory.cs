using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FurnitureInventory : MonoBehaviour
{
    public Dictionary<CostOfTheFurnitureEnum, uint> FurnitureQuantity { get; } = new();
    
    public void AddAmountRawMaterial(CostOfTheFurnitureEnum cost, uint quantity)
    {
        if (!FurnitureQuantity.ContainsKey(cost))
        {
            CreateRawMaterial(cost, quantity);
            return;
        }
        FurnitureQuantity[cost] += quantity;
    }
    
    public void RemoveAmountRawMaterial(CostOfTheFurnitureEnum cost, uint quantity)
    {
        if (!FurnitureQuantity.TryGetValue(cost, out var value))
        {
            return;
        }

        if (value <= quantity)
        {
            DestroyRawMaterial(cost);
            return;
        }
        FurnitureQuantity[cost] -= quantity;
    }

    private void CreateRawMaterial(CostOfTheFurnitureEnum cost, uint quantity)
    {
        FurnitureQuantity.Add(cost, quantity);
    }

    private void DestroyRawMaterial(CostOfTheFurnitureEnum cost)
    {
        FurnitureQuantity.Remove(cost);
    }
}
