using UnityEngine;
using UnityEngine.EventSystems;

public class SlotFurnitureRoom : MonoBehaviour ,IPointerClickHandler
{
    [SerializeField] private RoomSystem roomSystem;
    [SerializeField] private SizeOfFurniture typeSlot;
    
    [SerializeField] private GameObject uiListFurniture;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabFurniture;
    
    private FurnitureFeaturesSo furniture;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var kayValue in roomSystem.listOfFurnitureUnlocked[typeSlot])
        {
            if (roomSystem.prefabsFurniture.TryGetValue(kayValue.Key, out var value))
            {
                value.slot = this;
                value.gameObject.SetActive(true);
                continue;
            }
            GameObject inst = Instantiate(prefabFurniture, content.transform);

            if (!inst.TryGetComponent(out PrefebUiFurniture prefabInst))
            {
                Debug.LogError("No prefabUiFurniture found in this object or its parents");
                return;
            }

            prefabInst.SetTMP(roomSystem.GetNameFurniture(),roomSystem.GetDescriptionFurniture());
            
            prefabInst.furniture = kayValue.Key;
            prefabInst.slot = this;
            
            roomSystem.prefabsFurniture.Add(kayValue.Key, prefabInst);
        }
        
        uiListFurniture.SetActive(true);
    }

    public void SetFunctionary(FurnitureFeaturesSo newfurniture)
    {
        if (furniture is null)
        {
            roomSystem.RemoveUnlockedFurniture(typeSlot ,newfurniture);
            
            furniture = newfurniture;
            roomSystem.AddFurniture(furniture);
        }
        else if (furniture != newfurniture)
        {
            roomSystem.RemoveUnlockedFurniture(typeSlot ,newfurniture);
            roomSystem.AddUnlockedFurniture(typeSlot, furniture);

            roomSystem.RemoveFurniture(furniture);
            furniture = newfurniture;
            roomSystem.AddFurniture(furniture);
        }

        CloseUI();
    }

    public void CloseUI()
    {
        uiListFurniture.SetActive(false);
        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}

