using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages furniture slot interactions in a room.
/// Handles furniture placement, replacement, and UI display for available furniture options.
/// </summary>
public class SlotFurnitureRoom : MonoBehaviour, IPointerClickHandler
{
    #region Inspector Fields

    [Header("Room Configuration")]
    [SerializeField]
    [Tooltip("Reference to the room system that manages all furniture")]
    private RoomSystem roomSystem;
    
    [SerializeField]
    [Tooltip("Size category of this furniture slot (Small, Large, Well, Roof)")]
    public SizeOfFurnitureEnum slotType;
    
    [Header("UI References")]
    [SerializeField]
    [Tooltip("Prefab template for furniture UI list items")]
    private GameObject prefabFurniture;

    [Header("Furniture Spawning")]
    [SerializeField]
    [Tooltip("Transform parent where the 3D furniture instance will be spawned")]
    private GameObject furnitureInstanceSpawn;
    
    #endregion

    #region Private Fields
    
    /// <summary>Currently instantiated 3D furniture GameObject in the scene</summary>
    private GameObject _currentFurnitureInstance;
    
    /// <summary>Currently assigned furniture data for this slot</summary>
    private FurnitureFeaturesSo _currentFurniture;
    
    private RoomCanvasStatic roomCanvasSington;

    private GameObject uiListFurniture;
    
    private GameObject content;

    #endregion
    
    #region Unity Methods

    private void Start()
    {
        roomCanvasSington = RoomCanvasStatic.Instance;
        content = roomCanvasSington.Content;
        uiListFurniture = roomCanvasSington.PrefabFurniture;
    }

    #endregion

    #region Pointer Events
    
    /// <summary>
    /// Handles click events on the furniture slot.
    /// Opens the furniture selection UI and populates it with unlocked furniture options.
    /// </summary>
    /// <param name="eventData">Event data from the pointer click</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        PopulateFurnitureList();
        uiListFurniture.SetActive(true);
    }
    
    #endregion
    
    #region Furniture List Management
    
    /// <summary>
    /// Populates the UI list with all unlocked furniture for this slot type.
    /// Reuses existing UI prefabs if available, otherwise creates new ones.
    /// </summary>
    private void PopulateFurnitureList()
    {
        foreach (var keyValue in roomCanvasSington.ListOfFurnitureUnlocked[slotType])
        {
            // Reuse existing UI prefab if already created
            if (roomCanvasSington.PrefabsFurniture.TryGetValue(keyValue.Key, out var existingPrefab))
            {
                existingPrefab.slot = this;
                existingPrefab.gameObject.SetActive(true);
                continue;
            }
            
            // Create a new UI prefab for this furniture
            CreateFurnitureUIPrefab(keyValue.Key);
        }
    }
    
    /// <summary>
    /// Creates a new UI prefab for a furniture item and adds it to the cache.
    /// </summary>
    /// <param name="furniture">The furniture data to create a UI prefab for</param>
    private void CreateFurnitureUIPrefab(FurnitureFeaturesSo furniture)
    {
        GameObject instance = Instantiate(prefabFurniture, content.transform);

        if (!instance.TryGetComponent(out PrefabUiFurniture prefabComponent))
        {
            Debug.LogError("PrefabUiFurniture component not found on instantiated prefab");
            Destroy(instance);
            return;
        }

        // Configure the UI prefab with furniture data
        prefabComponent.SetTooltipTextComponents(roomSystem.GetNameFurniture(), roomSystem.GetDescriptionFurniture());
        prefabComponent.furniture = furniture;
        prefabComponent.slot = this;
        
        // Cache the prefab for reuse
        roomCanvasSington.AddPrefabsFurniture(furniture, prefabComponent);
    }
    
    #endregion
    
    #region Furniture Assignment

    /// <summary>
    /// Assigns or replaces furniture in this slot.
    /// Handles three scenarios: empty slot, replacing furniture, or re-selecting current furniture.
    /// </summary>
    /// <param name="newFurniture">The furniture data to assign to this slot</param>
    public void SetFurniture(FurnitureFeaturesSo newFurniture)
    {
        // Scenario 1: Empty slot - assign new furniture
        if (_currentFurniture == null)
        {
            AssignNewFurniture(newFurniture);
        }
        // Scenario 2: Different furniture - swap furniture
        else if (_currentFurniture != newFurniture)
        {
            ReplaceFurniture(newFurniture);
        }
        // Scenario 3: Same furniture - just close UI
        else
        {
            CloseUI();
            return;
        }
        
        // Instantiate 3D furniture model in the scene
        _currentFurnitureInstance = Instantiate(newFurniture.Prefab, furnitureInstanceSpawn.transform);
        
        CloseUI();
    }

    /// <summary>
    /// Assigns furniture to an empty slot.
    /// </summary>
    /// <param name="newFurniture">The furniture to assign</param>
    private void AssignNewFurniture(FurnitureFeaturesSo newFurniture)
    {
        roomCanvasSington.RemoveUnlockedFurniture(slotType, newFurniture);
        _currentFurniture = newFurniture;
        roomSystem.AddFurniture(_currentFurniture);
    }

    /// <summary>
    /// Replaces existing furniture with new furniture.
    /// Returns old furniture to the unlocked list and removes old instance.
    /// </summary>
    /// <param name="newFurniture">The furniture to replace with</param>
    private void ReplaceFurniture(FurnitureFeaturesSo newFurniture)
    {
        // Remove new furniture from the unlocked list
        roomCanvasSington.RemoveUnlockedFurniture(slotType, newFurniture);
        
        // Return old furniture to the unlocked list
        roomCanvasSington.AddUnlockedFurniture(slotType, _currentFurniture);

        // Update room system
        roomSystem.RemoveFurniture(_currentFurniture);
        roomSystem.AddFurniture(newFurniture);

        _currentFurniture = newFurniture;
    
        // Clean up old 3D instance
        if (_currentFurnitureInstance != null)
        {
            Destroy(_currentFurnitureInstance);
        }
    }

    public void LoadFurniture(FurnitureFeaturesSo newFurniture)
    {
        // Instantiate 3D furniture model in the scene
        _currentFurnitureInstance = Instantiate(newFurniture.Prefab, furnitureInstanceSpawn.transform);
        _currentFurniture = newFurniture;
    }
    
    
    #endregion
    
    #region UI Management

    /// <summary>
    /// Closes the furniture selection UI and hides all furniture list items.
    /// </summary>
    public void CloseUI()
    {
        uiListFurniture.SetActive(false);
        
        // Hide all child furniture UI items
        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    #endregion
}