using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject storeUI;
    
    private PlayerInteractionManager _playerInteractionManager;
    
    // singleton
    private RawMaterialStatic _rawMaterialStatic;
    private RoomCanvasStatic _roomCanvasStatic;

    [HideInInspector] public List<FurnitureFeaturesSo> allFurnitureFormProject = new();
    
    private void Start()
    {
        _rawMaterialStatic = RawMaterialStatic.Instance;
        _roomCanvasStatic = RoomCanvasStatic.Instance;
    }
    
    public void OpenStore(PlayerInteractionManager playerInteractionManager) 
    {
        _playerInteractionManager = playerInteractionManager;
        storeUI.SetActive(true);
    }
    
    public void CloseStore() 
    {
        _playerInteractionManager.EndInteraction();
        storeUI.SetActive(false);
    }

    public List<(CostOfTheFurnitureEnum ,uint,uint)> GetCostForFurniture(FurnitureFeaturesSo furnitureFeaturesSo)
    {
        if(furnitureFeaturesSo is null) return null;
        
        var allCosts= furnitureFeaturesSo.GetAllCosts();

        List<(CostOfTheFurnitureEnum, uint,uint)> list = new();
        var rawMaterialAmount = _rawMaterialStatic.GetFurnitureQuantity();

        foreach (var (key, value) in allCosts)
        {
            list.Add(rawMaterialAmount.TryGetValue(key, out var amount)
                ? (key, value.value, amount)
                : ((CostOfTheFurnitureEnum, uint, uint))(key, value.value, 0));
        }
        
        return list;
    }

    public Task BuyFurniture(FurnitureFeaturesSo furnitureFeaturesSo)
    {
        if(furnitureFeaturesSo is null) return Task.CompletedTask;
        
        var allCosts= furnitureFeaturesSo.GetAllCosts();

        foreach (var (key, value) in allCosts)
        {
            _rawMaterialStatic.RemoveMaterialAmount(key, value.value);
        }
        
        _roomCanvasStatic.AddUnlockedFurniture(furnitureFeaturesSo.Size, furnitureFeaturesSo);
        
        return Task.CompletedTask;
    }
}
