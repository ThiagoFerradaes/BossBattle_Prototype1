using System;
using MyEnum;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages room entry interactions, door states, and editor UI for tavern rooms.
/// Handles player entering/exiting triggers and language localization for UI elements.
/// </summary>
public class RoomEnterable : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("Door Settings")]
    [SerializeField]
    [Tooltip("Reference to the door GameObject that will be shown/hidden")]
    private GameObject door;
    
    [SerializeField]
    [Tooltip("Initial state of the door (true = open, false = closed)")]
    private bool isDoorOpen;
    
    [Header("Player Detection")]
    [SerializeField]
    [Tooltip("Layer mask to identify player objects")]
    private LayerMask playerLayer;
    
    [Header("Editor UI")]
    [SerializeField]
    [Tooltip("UI GameObject shown when player can interact with the room")]
    private GameObject editorUI;
    
    [SerializeField]
    [Tooltip("Text component displaying interaction prompts")]
    private TMP_Text editorRoomText;
    
    [Header("Localization")]
    [SerializeField]
    [Tooltip("Localized text for opening the editor")]
    private TextBoxesSo openEditorButtonText;
    
    [SerializeField]
    [Tooltip("Localized text for closing the editor")]
    private TextBoxesSo closeEditorButtonText;
    
    #endregion

    #region Private Fields
    
    /// <summary>Cached localized text for opening editor button</summary>
    private string _openEditorButton;
    
    /// <summary>Cached localized text for closing editor button</summary>
    private string _closeEditorButton;
    
    /// <summary>Current enabled state of the room</summary>
    private bool _enableRoom;
    
    /// <summary>Reference to the player's interaction manager component</summary>
    private PlayerInteractionManager _playerInteractionManager;
    
    /// <summary>Tracks whether the room editor is currently open</summary>
    private bool _isEditorOpen;
    
    /// <summary>Reference to the game configuration for language settings</summary>
    private ConfigurationSo _config;
    
    #endregion

    #region Events
    
    /// <summary>Event triggered when a player enters the room</summary>
    public event Action<RoomEnterable> OnRoomEntered;
    
    #endregion
    
    #region Unity Lifecycle Methods
    
    /// <summary>
    /// Initialize room state and configuration on enabler
    /// </summary>
    private void OnEnable()
    {
        SetEnableRoom(isDoorOpen);
        InitializeConfiguration();
    }
    
    /// <summary>
    /// Cleanup event subscriptions on disable
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Localization
    
    /// <summary>
    /// Loads configuration and subscribes to language change events
    /// </summary>
    private void InitializeConfiguration()
    {
        if (_config == null)
        {
            _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");
        }

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }
    
    /// <summary>
    /// Unsubscribes from language change events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (_config != null)
        {
            _config.OnLanguageChanged -= UpdateLanguage;
        }
    }
    
    /// <summary>
    /// Updates UI text based on the selected language
    /// </summary>
    /// <param name="lang">Target language enum</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        _openEditorButton = openEditorButtonText.GetText(lang);
        _closeEditorButton = closeEditorButtonText.GetText(lang);
    }

    #endregion
    
    #region Trigger Detection

    /// <summary>
    /// Detects when a collider enters the room trigger zone
    /// </summary>
    /// <param name="other">The collider that entered</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsInLayerMask(playerLayer, other.gameObject)) return;
        
        PlayerEntered(other);
    }

    /// <summary>
    /// Detects when a collider exits the room trigger zone
    /// </summary>
    /// <param name="other">The collider that exited</param>
    private void OnTriggerExit(Collider other)
    {
        if (!IsInLayerMask(playerLayer, other.gameObject)) return;
        
        PlayerExited(other);
    }
    
    #endregion
    
    #region Player Interaction
    
    /// <summary>
    /// Handles player entering the room trigger
    /// Shows UI and subscribes to interaction events
    /// </summary>
    /// <param name="other">The player collider</param>
    private void PlayerEntered(Collider other)
    {
        if (_playerInteractionManager != null) return;

        if (!other.TryGetComponent(out _playerInteractionManager))
        {
            return;
        }
        
        editorUI.SetActive(true);
        editorRoomText.text = _openEditorButton;
        _playerInteractionManager.SetRoomEnterable(this);
        _playerInteractionManager.OnEditorInteraction += ToggleEditor;
    }

    /// <summary>
    /// Handles player exiting the room trigger
    /// Hides UI and unsubscribes from interaction events
    /// </summary>
    /// <param name="other">The player collider</param>
    private void PlayerExited(Collider other)
    {
        if (_playerInteractionManager == null) return;
        if (_playerInteractionManager.gameObject != other.gameObject) return;
        
        _isEditorOpen = false;
        editorUI.SetActive(false);
        _playerInteractionManager.OnEditorInteraction -= ToggleEditor;
        _playerInteractionManager.SetRoomEnterable(null);
        _playerInteractionManager = null;
    }
    
    /// <summary>
    /// Toggles the room editor on/off and updates the camera view
    /// </summary>
    private void ToggleEditor()
    {
        _isEditorOpen = !_isEditorOpen;
        TavernCameraController.Instance.ChangeCamera();
        editorRoomText.text = _isEditorOpen ? _closeEditorButton : _openEditorButton;
    }
    
    #endregion
    
    #region Door Management
    
    /// <summary>
    /// Gets the current enabled state of the room
    /// </summary>
    /// <returns>True if room is enabled, false otherwise</returns>
    public bool GetEnableRoom()
    {
        return _enableRoom;
    }

    /// <summary>
    /// Sets the enabled state of the room and controls door visibility
    /// </summary>
    /// <param name="enableRoom">Target enabled state</param>
    public void SetEnableRoom(bool enableRoom)
    {
        if (_enableRoom == enableRoom) return;
        
        OnRoomEntered?.Invoke(this);
        _enableRoom = enableRoom;
        door.SetActive(!_enableRoom);
    }
    
    #endregion

    #region Utility Methods
    
    /// <summary>
    /// Checks if a GameObject's layer is included in a LayerMask
    /// </summary>
    /// <param name="layerMask">The LayerMask to check against</param>
    /// <param name="gameObject">The GameObject to check</param>
    /// <returns>True if the GameObject's layer is in the LayerMask</returns>
    private static bool IsInLayerMask(LayerMask layerMask, GameObject gameObject)
    {
        return (layerMask.value & (1 << gameObject.layer)) != 0;
    }
    
    #endregion
}