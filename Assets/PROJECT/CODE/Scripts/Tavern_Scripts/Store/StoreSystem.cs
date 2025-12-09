using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using MyEnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreSystem : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject storeUI;
    [SerializeField] private GameObject preferItemBuy;
    [SerializeField] private RectTransform storeContent;
    
    [Header("TexBox")]
    [SerializeField] private UITextLocalizer nameItem;
    [SerializeField] private UITextLocalizer descriptionItem;
    
    [Header("image")]
    [SerializeField] private Image imageItem;
    
    [Header("Dictionary")]
    [SerializedDictionary("Type", "Info")]
    [SerializeField] private SerializedDictionary<TypeOfEnvironmentCharacteristicEnum, GameObject> itemsBySize = new();
    
    [Header("Button")]
    [SerializeField] private Button buyButton;
    
    [Header("Cost")]
    [SerializeField] private List<CostAndAmount> costAndAmount = new();
    
    private PlayerInteractionManager _playerInteractionManager;
    
    // singleton
    private RawMaterialStatic _rawMaterialStatic;
    private RoomCanvasStatic _roomCanvasStatic;

    public FurnitureFeaturesSo[] allFurnitureFormProject;
    

    private async void Start()
    {
        try
        {
            _rawMaterialStatic = RawMaterialStatic.Instance;
            _roomCanvasStatic = RoomCanvasStatic.Instance;

            await SpawnItem();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    private Task SpawnItem()
    {
        List<(TypeOfEnvironmentCharacteristicEnum, GameObject)> furnitureToBuy = itemsBySize.Select(variable => (variable.Key, variable.Value)).ToList();
        foreach (var furnitureFeaturesSo in allFurnitureFormProject)
        {
            var item = Instantiate(preferItemBuy, storeContent);
            
            item.GetComponent<ItemSlotBuy>().SetItem(furnitureFeaturesSo, descriptionItem, nameItem, imageItem, furnitureToBuy, costAndAmount, buyButton, this);
        }
        return Task.CompletedTask;
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

[Serializable]
public class CostAndAmount
{
    public GameObject gameObject;
    public Image image;
    public TMP_Text cost;
    public TMP_Text amount;   
}