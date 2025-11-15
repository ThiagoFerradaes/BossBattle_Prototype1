using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrefebUiFurniture : MonoBehaviour ,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SlotFurnitureRoom slot;
    public FurnitureFeaturesSo furniture;

    private TMP_Text nameFurniture, descriptionFurniture;

    private ConfigurationSo _config;

    public void SetTMP(TMP_Text nameTmp, TMP_Text descriptionTmp)
    {
        nameFurniture = nameTmp;
        descriptionFurniture = descriptionTmp;
    }
    
    private void Start()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
    }

    private void OnEnable()
    {
        nameFurniture.text = "";
        descriptionFurniture.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        slot.SetFunctionary(furniture);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var enumLanguage = _config.GetLanguage();
        nameFurniture.text = furniture.Name.GetText(enumLanguage);
        descriptionFurniture.text = furniture.Description.GetText(enumLanguage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameFurniture.text = "";
        descriptionFurniture.text = "";
    }
}
