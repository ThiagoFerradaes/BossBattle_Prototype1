using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the tavern camera system, controlling transitions between persona and editor views.
/// Implements the singleton pattern and handles camera positioning based on player movement.
/// </summary>
public class TavernCameraController : MonoBehaviour
{
    #region Singleton
    
    /// <summary>Singleton instance of the TavernCameraController</summary>
    public static TavernCameraController Instance { get; private set; }
    
    #endregion

    #region Inspector Fields
    
    [Header("Cinemachine Cameras")]
    [SerializeField]
    [Tooltip("Cinemachine camera used for persona/player view")]
    private CinemachineCamera personaCamera;
    
    [SerializeField]
    [Tooltip("Cinemachine camera used for editor/build view")]
    private CinemachineCamera editorCamera;
    
    [Header("Raycaster Settings")]
    [SerializeField]
    [Tooltip("Physics raycaster for detecting UI interactions")]
    private PhysicsRaycaster raycaster;
    
    [Header("Layer Masks")]
    [SerializeField]
    [Tooltip("Layer mask for all interactive layers in persona mode")]
    private LayerMask allLayers;
    
    [SerializeField]
    [Tooltip("Layer mask for buildable layers in editor mode")]
    private LayerMask buildLayers;
    
    [Header("Input Settings")]
    [SerializeField]
    [Tooltip("Input action reference for camera movement in editor mode")]
    private InputActionReference moveActionReference;
    
    [SerializeField]
    [Tooltip("Speed of camera movement in editor mode")]
    private float cameraSpeed = 10f;
    
    #endregion

    #region Events
    
    /// <summary>Event triggered when camera mode changes between persona and editor</summary>
    public event Action OnCameraChanged;
    
    #endregion

    #region Private Fields
    
    /// <summary>Reference to the player's transform for camera tracking</summary>
    private Transform _playerTransform;
    
    /// <summary>Tracks if the camera is currently in persona mode</summary>
    private bool _isPersonaMode = true;
    
    /// <summary>Z-axis distance offset for persona camera</summary>
    private float _personaCameraZOffset;
    
    /// <summary>Z-axis distance offset for editor camera</summary>
    private float _editorCameraZOffset;
    
    /// <summary>Y-axis distance offset for both cameras</summary>
    private float _cameraYOffset;
    
    /// <summary>Cached transform of persona camera</summary>
    private Transform _personaCameraTransform;
    
    /// <summary>Cached transform of editor camera</summary>
    private Transform _editorCameraTransform;
    
    #endregion
    
    #region Unity Lifecycle Methods
    
    /// <summary>
    /// Initializes singleton instance and calculates camera offset distances
    /// </summary>
    private void Awake()
    {
        InitializeSingleton();
        CacheTransforms();
        CalculateCameraOffsets();
    }

    /// <summary>
    /// Updates camera position based on current mode and player position
    /// </summary>
    private void Update()
    {
        if (_playerTransform == null) return;
        
        if (_isPersonaMode)
        {
            UpdatePersonaCameraPosition();
        }
        else
        {
            UpdateEditorCameraPosition();
        }
    }
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// Initializes singleton pattern, destroying duplicates
    /// </summary>
    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    /// <summary>
    /// Caches transform references for performance optimization
    /// </summary>
    private void CacheTransforms()
    {
        _personaCameraTransform = personaCamera.transform;
        _editorCameraTransform = editorCamera.transform;
    }
    
    /// <summary>
    /// Calculates initial camera offset distances from the origin
    /// </summary>
    private void CalculateCameraOffsets()
    {
        Vector3 personaPos = _personaCameraTransform.position;
        Vector3 originPos = transform.position;
        
        _personaCameraZOffset = personaPos.z - originPos.z;
        _editorCameraZOffset = _editorCameraTransform.position.z - originPos.z;
        _cameraYOffset = personaPos.y - originPos.y;
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Checks if the persona camera is currently active
    /// </summary>
    /// <returns>True if persona camera is active, false otherwise</returns>
    public bool IsPersonaCameraActive()
    {
        return personaCamera.Priority == 1;
    }
    
    /// <summary>
    /// Toggles between persona and editor camera modes
    /// </summary>
    public void ChangeCamera()
    {
        OnCameraChanged?.Invoke();
        
        if (_isPersonaMode)
        {
            SwitchToEditorMode();
        }
        else
        {
            SwitchToPersonaMode();
        }
    }
    
    /// <summary>
    /// Sets the player transform reference for camera tracking
    /// </summary>
    /// <param name="playerTransform">Transform of the player object</param>
    public void SetPlayerTransform(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Switches camera to persona mode and updates raycaster mask
    /// </summary>
    private void SwitchToPersonaMode()
    {
        _isPersonaMode = true;
        personaCamera.Priority = 1;
        editorCamera.Priority = 0;
        raycaster.eventMask = allLayers;
    }
    
    /// <summary>
    /// Switches camera to editor mode, positions editor camera, and updates raycaster mask
    /// </summary>
    private void SwitchToEditorMode()
    {
        if (_playerTransform != null)
        {
            Vector3 playerPos = _playerTransform.position;
            _editorCameraTransform.position = new Vector3(
                playerPos.x,
                _cameraYOffset,
                playerPos.z + _editorCameraZOffset
            );
        }
        
        _isPersonaMode = false;
        personaCamera.Priority = 0;
        editorCamera.Priority = 1;
        raycaster.eventMask = buildLayers;
    }
    
    /// <summary>
    /// Updates persona camera position to follow player
    /// </summary>
    private void UpdatePersonaCameraPosition()
    {
        Vector3 playerPos = _playerTransform.position;
        _personaCameraTransform.position = new Vector3(
            playerPos.x,
            _cameraYOffset,
            playerPos.z + _personaCameraZOffset
        );
    }
    
    /// <summary>
    /// Updates editor camera position based on input movement
    /// </summary>
    private void UpdateEditorCameraPosition()
    {
        Vector2 inputValue = moveActionReference.action.ReadValue<Vector2>();
        Vector2 movement = inputValue * (cameraSpeed * Time.deltaTime);
        
        Vector3 currentPos = _editorCameraTransform.position;
        _editorCameraTransform.position = new Vector3(
            currentPos.x + movement.x,
            _cameraYOffset,
            currentPos.z + movement.y
        );
    }
    
    #endregion
}