using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the inventory of furniture materials/resources.
/// Tracks quantities of different material types used for furniture crafting and upgrades.
/// </summary>
public class FurnitureInventory : MonoBehaviour
{
    #region Properties
    
    /// <summary>
    /// Dictionary containing the quantity of each furniture material type.
    /// Key: Material type enum
    /// Value: Quantity available
    /// </summary>
    public Dictionary<CostOfTheFurnitureEnum, uint> FurnitureQuantity { get; } = new();
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Adds a specified quantity of material types to the inventory.
    /// If the material doesn't exist in the inventory, it creates a new entry.
    /// </summary>
    /// <param name="materialType">The type of material to add</param>
    /// <param name="quantity">The amount to add (must be positive)</param>
    public void AddMaterialAmount(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        if (!FurnitureQuantity.ContainsKey(materialType))
        {
            CreateMaterial(materialType, quantity);
            return;
        }
        
        FurnitureQuantity[materialType] += quantity;
    }
    
    /// <summary>
    /// Removes a specified quantity of material types from the inventory.
    /// If the quantity to remove is greater than or equal to the available amount,
    /// the material entry is completely removed from the inventory.
    /// </summary>
    /// <param name="materialType">The type of material to remove</param>
    /// <param name="quantity">The amount to remove</param>
    public void RemoveMaterialAmount(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        if (!FurnitureQuantity.TryGetValue(materialType, out var currentQuantity))
        {
            return;
        }

        if (currentQuantity <= quantity)
        {
            DestroyMaterial(materialType);
            return;
        }
        
        FurnitureQuantity[materialType] -= quantity;
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Creates a new material entry in the inventory with the specified quantity.
    /// </summary>
    /// <param name="materialType">The type of material to create</param>
    /// <param name="quantity">The initial quantity</param>
    private void CreateMaterial(CostOfTheFurnitureEnum materialType, uint quantity)
    {
        FurnitureQuantity.Add(materialType, quantity);
    }

    /// <summary>
    /// Removes a material entry completely from the inventory.
    /// </summary>
    /// <param name="materialType">The type of material to remove</param>
    private void DestroyMaterial(CostOfTheFurnitureEnum materialType)
    {
        FurnitureQuantity.Remove(materialType);
    }
    
    #endregion
}