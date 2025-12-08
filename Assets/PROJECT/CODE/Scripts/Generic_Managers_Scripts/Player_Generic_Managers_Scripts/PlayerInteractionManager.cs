using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/// <summary>
/// Manages player interactions with objects in the game world, including dialogues,
/// maps, and room editor functionality. Handles input actions and interaction detection
/// using sphere casting within a defined range.
/// </summary>
public class PlayerInteractionManager : MonoBehaviour
{
    #region Events
    
    /// <summary>Event triggered when the player activates the editor interaction</summary>
    public event Action OnEditorInteraction;
    
    #endregion

    #region Inspector Fields
    
    [Header("Interaction Settings")]
    [SerializeField]
    [Tooltip("Maximum distance the player can interact with objects")]
    private float interactionRange = 2.0f;

    [SerializeField]
    [Tooltip("Layer mask defining which layers contain interactable objects")]
    private LayerMask interactionLayer;
    
    [Header("Dependencies")]
    [SerializeField]
    [Tooltip("Reference to the player movement manager component")]
    private PlayerMovementManager playerMovementManager;
    
    #endregion

    #region Private Fields
    
    /// <summary>Tracks if interaction input is currently pressed</summary>
    private bool _isInteracting;
    
    /// <summary>Indicates whether a player is paused during an interaction</summary>
    private bool _isPaused;
    
    /// <summary>Reference to the dialogue system for conversation handling</summary>
    private DialogueSystem _dialogueSystem;
    
    /// <summary>Reference to the map manager for map display</summary>
    private MapManager _mapManager;
    
    /// <summary>Reference to the current enterable room, if any</summary>
    private RoomEnterable _roomEnterable;
    
    /// <summary>Button reference for editor room interactions</summary>
    private Button _editorRoomButton;

    /// <summary>Tracks the current camera state (persona camera active/inactive)</summary>
    private bool _isPersonaCameraActive;
    
    private StoreSystem _store;
    
    #endregion
    
    #region Unity Lifecycle Methods
    
    /// <summary>
    /// Subscribe to tavern loading events
    /// </summary>
    private void OnEnable()
    {
        CanvasTavernaManager.OnTavernaLoaded += InitializeTavernReferences;
    }

    /// <summary>
    /// Cleanup event subscriptions
    /// </summary>
    private void OnDisable()
    {
        if (TavernCameraController.Instance != null)
        {
            TavernCameraController.Instance.OnCameraChanged -= OnCameraChanged;
        }
        
        if (_editorRoomButton != null)
        {
            _editorRoomButton.onClick.RemoveListener(OnEditorInteractionEvent);
        }
    }

    /// <summary>
    /// Performs interaction detection using sphere casting.
    /// Called in FixedUpdate for consistent physics checks.
    /// </summary>
    private void FixedUpdate()
    {
        if (!CanInteract()) return;

        if (DetectInteractableObject(out InteractiveObject interactiveObject))
        {
            ExecuteInteraction(interactiveObject);
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initializes references from the tavern manager and sets up event listeners
    /// </summary>
    private void InitializeTavernReferences()
    {
        if (CanvasTavernaManager.Instance == null) return;

        _dialogueSystem = CanvasTavernaManager.Instance.DialogueSystem;
        _mapManager = CanvasTavernaManager.Instance.MapManager;
        _editorRoomButton = CanvasTavernaManager.Instance.EditorRoomButton;
        _store = CanvasTavernaManager.Instance.StoreSystem;
        
        if (TavernCameraController.Instance != null)
        {
            TavernCameraController.Instance.OnCameraChanged += OnCameraChanged;
            TavernCameraController.Instance.SetPlayerTransform(transform);
        }
        
        if (_editorRoomButton != null)
        {
            _editorRoomButton.onClick.AddListener(OnEditorInteractionEvent);
        }
        
        CanvasTavernaManager.OnTavernaLoaded -= InitializeTavernReferences;
    }
    
    #endregion
    
    #region Interaction Detection
    
    /// <summary>
    /// Checks if the player can currently interact with objects
    /// </summary>
    /// <returns>True if all conditions for interaction are met</returns>
    private bool CanInteract()
    {
        return _dialogueSystem != null && _isInteracting && !_isPaused;
    }
    
    /// <summary>
    /// Detects interactable objects using sphere casting
    /// </summary>
    /// <param name="interactiveObject">The detected interactive object, if any</param>
    /// <returns>True if an interactive object was found</returns>
    private bool DetectInteractableObject(out InteractiveObject interactiveObject)
    {
        interactiveObject = null;
        
        const float sphereRadius = 0.1f;
        if (!Physics.SphereCast(transform.position, sphereRadius, transform.forward, 
            out RaycastHit hit, interactionRange, interactionLayer))
        {
            return false;
        }

        if (!hit.collider.gameObject.TryGetComponent(out interactiveObject))
        {
            Debug.LogWarning("Hit object does not contain an InteractiveObject component");
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Executes the interaction with the detected object
    /// </summary>
    /// <param name="interactiveObject">The object to interact with</param>
    private async void ExecuteInteraction(InteractiveObject interactiveObject)
    {
        try
        {
            _isInteracting = false;
            SetPlayerMovementState(true);
            _isPaused = true;
            
            await interactiveObject.interaction.Execute(_dialogueSystem, this);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error executing interaction: {e.Message}\n{e.StackTrace}");
            EndInteraction();
        }
    }
    
    #endregion
    
    #region Player Movement Control
    
    /// <summary>
    /// Sets the player movement state (blocked or unblocked)
    /// </summary>
    /// <param name="blocked">True to block movement, false to allow it</param>
    private void SetPlayerMovementState(bool blocked)
    {
        if (playerMovementManager == null) return;
        
        playerMovementManager.BlockWalk(blocked);
        playerMovementManager.BlockMovement(blocked);
    }
    
    #endregion
    
    #region Dialogue Management
    
    /// <summary>
    /// Called when dialogue sequence completes
    /// </summary>
    /// <param name="obj">The dialogue system scriptable object that completed</param>
    public void EndDialogue(DialogueSystemSo obj)
    {
        if (_dialogueSystem != null)
        {
            _dialogueSystem.OnComplicitEvent -= EndDialogue;
        }
        EndInteraction();
    }
    
    #endregion
    
    #region Map Management
    
    /// <summary>
    /// Opens the map interface and subscribes to close events
    /// </summary>
    public void OpenMap()
    {
        if (_mapManager == null)
        {
            Debug.LogWarning("MapManager reference is null. Cannot open map.");
            return;
        }
        
        Debug.Log("Opening map interface");
        _mapManager.OnCloseMap += OnMapClosed;
        _mapManager.gameObject.SetActive(true);
    }

    public void StoreOpen()
    {
        _store.OpenStore(this);
    }
    
    /// <summary>
    /// Handles map closing event
    /// </summary>
    private void OnMapClosed()
    {
        if (_mapManager != null)
        {
            _mapManager.OnCloseMap -= OnMapClosed;
        }
        EndInteraction();
    }
    
    #endregion
    
    #region Interaction State Management
    
    /// <summary>
    /// Ends the current interaction and re-enables player movement
    /// </summary>
    public void EndInteraction()
    {
        _isPaused = false;
        SetPlayerMovementState(false);
    }
    
    #endregion
    
    #region Input Handling
    
    /// <summary>
    /// Handles interaction input from the Input System
    /// </summary>
    /// <param name="context">Input action callback context</param>
    public void Interaction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isInteracting = true;
        }
        else if (context.canceled)
        {
            _isInteracting = false;
        }
    }
    
    /// <summary>
    /// Handles editor interaction input when inside a room
    /// </summary>
    /// <param name="context">Input action callback context</param>
    public void EditorInteractionMap(InputAction.CallbackContext context)
    {
        if (_roomEnterable == null) return;
        
        if (context.started)
        {
            OnEditorInteractionEvent();
        }
    }
    
    /// <summary>
    /// Triggers the editor interaction event
    /// </summary>
    public void OnEditorInteractionEvent()
    {
        OnEditorInteraction?.Invoke();
    }
    
    #endregion

    #region Room Management
    
    /// <summary>
    /// Sets the current enterable room reference
    /// </summary>
    /// <param name="roomEnterable">The room that can be entered or null to clear</param>
    public void SetRoomEnterable(RoomEnterable roomEnterable)
    {
        _roomEnterable = roomEnterable;
    }
    
    #endregion
    
    #region Camera Management
    
    /// <summary>
    /// Handles camera state changes and updates room editor state
    /// </summary>
    private void OnCameraChanged()
    {
        if (TavernCameraController.Instance == null) return;
        
        _isPersonaCameraActive = TavernCameraController.Instance.IsPersonaCameraActive();
        
        if (playerMovementManager != null)
        {
            playerMovementManager.RoomEditor(_isPersonaCameraActive);
        }
    }
    
    #endregion
}