using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI prefab controller for furniture items in the tavern furniture selection system.
/// Handles displaying furniture information, responding to pointer events (click, hover),
/// and managing localized text for furniture names and descriptions.
/// </summary>
public class PrefabUiFurniture : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Inspector Fields
    
    [Header("References")]
    [Tooltip("Reference to the furniture slot that will receive the selected furniture")]
    public SlotFurnitureRoom slot;
    
    [Tooltip("Scriptable object containing furniture data (name, description, art, etc.)")]
    public FurnitureFeaturesSo furniture;

    [Header("UI Elements")]
    [SerializeField]
    [Tooltip("Text component displaying the furniture name in the UI prefab")]
    private TMP_Text nameUi;
    
    [SerializeField]
    [Tooltip("Image component displaying the furniture sprite/icon")]
    private Image imageUi;
    
    #endregion

    #region Private Fields
    
    /// <summary>Text component for displaying furniture name in the tooltip /hover area</summary>
    private TMP_Text _nameFurniture;
    
    /// <summary>Text component for displaying furniture description in the tooltip /hover area</summary>
    private TMP_Text _descriptionFurniture;
    
    /// <summary>Cached reference to game configuration for language settings</summary>
    private ConfigurationSo _config;
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// Sets the tooltip text components for displaying furniture information on hover.
    /// Should be called during prefab instantiation.
    /// </summary>
    /// <param name="nameTmp">Text component for furniture name tooltip</param>
    /// <param name="descriptionTmp">Text component for furniture description tooltip</param>
    public void SetTooltipTextComponents(TMP_Text nameTmp, TMP_Text descriptionTmp)
    {
        _nameFurniture = nameTmp;
        _descriptionFurniture = descriptionTmp;
    }
    
    /// <summary>
    /// Loads game configuration on the start
    /// </summary>
    private void Start()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
    }

    #endregion

    #region Unity Lifecycle Methods
    
    /// <summary>
    /// Initializes UI elements when the prefab is enabled.
    /// Clears tooltip text and updates furniture name and icon based on the current language.
    /// </summary>
    private void OnEnable()
    {
        ClearTooltipText();
        
        // Ensure the configuration is loaded
        _config ??= Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
        
        UpdateFurnitureUI();
    }

    #endregion

    #region UI Updates
    
    /// <summary>
    /// Updates the furniture UI elements (name and icon) based on the current language
    /// </summary>
    private void UpdateFurnitureUI()
    {
        if (_config == null || furniture == null) return;
        
        var currentLanguage = _config.GetLanguage();
        nameUi.text = furniture.Name.GetText(currentLanguage);
        imageUi.sprite = furniture.Art;
    }
    
    /// <summary>
    /// Clears the tooltip text components
    /// </summary>
    private void ClearTooltipText()
    {
        if (_nameFurniture != null)
            _nameFurniture.text = "";
        
        if (_descriptionFurniture != null)
            _descriptionFurniture.text = "";
    }
    
    /// <summary>
    /// Updates tooltip with localized furniture name and description
    /// </summary>
    private void UpdateTooltipText()
    {
        if (_config == null || furniture == null) return;
        
        var currentLanguage = _config.GetLanguage();
        
        if (_nameFurniture != null)
            _nameFurniture.text = furniture.Name.GetText(currentLanguage);
        
        if (_descriptionFurniture != null)
            _descriptionFurniture.text = furniture.Description.GetText(currentLanguage);
    }
    
    #endregion

    #region Pointer Event Handlers
    
    /// <summary>
    /// Handles pointer click event - assigns selected furniture to the slot
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity Event System</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot != null && furniture != null)
        {
            slot.SetFurniture(furniture);
        }
    }

    /// <summary>
    /// Handles pointer enter event - displays furniture details in the tooltip
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity Event System</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateTooltipText();
    }

    /// <summary>
    /// Handles pointer exit event - clears tooltip text
    /// </summary>
    /// <param name="eventData">Pointer event data from Unity Event System</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ClearTooltipText();
    }
    
    #endregion
}