using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotBuy : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    private Image imageItemForBay;
    
    public UITextLocalizer itemName;
    private UITextLocalizer itemNameForBuy;
    private UITextLocalizer itemDescriptionForBuy;
    
    public UITextLocalizer typeOfItem;
    public UITextLocalizer typeOfItemRarity;

    private List<(TypeOfEnvironmentCharacteristicEnum, GameObject)> type = new();
    
    private List<CostAndAmount> cost = new();
    
    private FurnitureFeaturesSo furnitureFeaturesSo;
    
    private Button buyButton;

    private StoreSystem storeSystem;
    
    private bool isBuyButtonActive;
    
    [SerializeField] Color colorActive = Color.white;
    [SerializeField] Color colorInactive = Color.red;
    
    public void SetItem(FurnitureFeaturesSo getFurniture, UITextLocalizer description, UITextLocalizer nameItem, Image image, List<(TypeOfEnvironmentCharacteristicEnum, GameObject)> gettype, List<CostAndAmount> getCost, Button buy, StoreSystem system)
    {
        furnitureFeaturesSo = getFurniture;
        itemNameForBuy = nameItem;
        itemDescriptionForBuy = description;
        imageItemForBay = image;

        buyButton = buy;
        storeSystem = system;
        //set name and sprite

        itemImage.sprite = furnitureFeaturesSo.Art;
        itemName.SetTexBox(getFurniture.Name);

        typeOfItem.SetTexBox(ItemDB.GetName(getFurniture.Size));
        typeOfItemRarity.SetTexBox(ItemDB.GetName(getFurniture.Rarity));

        type = gettype;
        cost = getCost;
    }
    
    public async void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            await ConfigUI();
            buyButton.onClick.RemoveAllListeners();
        
            if(isBuyButtonActive)
                buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private Task ConfigUI()
    {
        imageItemForBay.sprite = furnitureFeaturesSo.Art;
        itemNameForBuy.SetTexBox(furnitureFeaturesSo.Name);
        itemDescriptionForBuy.SetTexBox(furnitureFeaturesSo.Description);
        
        var allCosts = furnitureFeaturesSo.GetAllCosts();
        var allCharacteristics = furnitureFeaturesSo.GetAllCharacteristics();
        
        foreach (var (typeOfEnvironmentCharacteristicEnum, image) in type)
        {
            image.gameObject.SetActive(allCharacteristics.ContainsKey(typeOfEnvironmentCharacteristicEnum));
        }

        foreach (var variable in cost)
        {
            variable.gameObject.SetActive(false);
        }
        
        var noCost = true;
        byte index = 0;
        foreach (var (kay, value) in allCosts)
        {
            cost[index].image.sprite = ItemDB.GetIcon(kay);

            var a = RawMaterialStatic.Instance.GetFurnitureQuantity();
            
            var rawMaterialAmount = a.TryGetValue(kay, out var value1) ? value1 : 0;
            
            if(noCost) noCost = value.value <= rawMaterialAmount;
            
            cost[index].cost.text = value.value.ToString();
            cost[index].amount.text = rawMaterialAmount.ToString();
            cost[index].gameObject.SetActive(true);
            index++;
        }
        
        isBuyButtonActive = noCost;
        
        buyButton.interactable = noCost;
        
        buyButton.image.color = isBuyButtonActive ? colorActive : colorInactive;
        
        return Task.CompletedTask;
    }
    
    private async void OnBuyButtonClicked()
    {
        try
        {
            await storeSystem.BuyFurniture(furnitureFeaturesSo);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
