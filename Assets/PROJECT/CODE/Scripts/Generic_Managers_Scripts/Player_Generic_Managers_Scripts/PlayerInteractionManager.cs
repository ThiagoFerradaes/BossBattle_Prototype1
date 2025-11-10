using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PlayerInteractionManager : MonoBehaviour
{
    private bool _inputActionsEnabled;
    
    private bool _isInteracting;
    
    private bool _isPaused;
    
    [SerializeField, Tooltip("The player's interaction range")]
    private float interactionRange;

    private DialogueSystem _dialogueSystem;
    
    private MapManager _mapManager;
    
    [SerializeField, Tooltip("The player's interaction LayerMask")]
    private LayerMask interactionLayer;
    
    [SerializeField,Tooltip("PlayerMovementManager reference")]
    private PlayerMovementManager playerMovementManager;
    
    private RoomEnterable _roomEnterable;
    private Button editorRoomButton;

    private bool cameraInfo;
    public event Action OnEditorInteraction;
    
    private void OnEnable()
    {
        CanvasTavernaManager.OnTavernaLoaded += CanvasTavernaManager_OnDisable;
    }

    private void OnDisable()
    {
        CameraCenterTaverna.Instance.OnCameraChanged -= ChangeCamera;
    }

    private void CanvasTavernaManager_OnDisable()
    {
        _dialogueSystem = CanvasTavernaManager.Instance.DialogueSystem;
        _mapManager = CanvasTavernaManager.Instance.MapManager;
        editorRoomButton = CanvasTavernaManager.Instance.EditorRoomButton;
        CameraCenterTaverna.Instance.OnCameraChanged += ChangeCamera;
        CameraCenterTaverna.Instance.SetPlayerTransform(transform);
        editorRoomButton.onClick.AddListener(OnEditorInteractionEvent);
        CanvasTavernaManager.OnTavernaLoaded -= CanvasTavernaManager_OnDisable;
    }
    
    private async void FixedUpdate()
    {
        try
        {
            if(_dialogueSystem is null) return;
            if(!_isInteracting) return;
            if(_isPaused) return;

            //Physics.Raycast(transform.position, transform.forward, our hit, interactionRange, interactionLayer);
            if (!Physics.SphereCast(transform.position, 0.1f, transform.forward, out var hit, interactionRange,
                    interactionLayer))
                return;

            if (!hit.collider.gameObject.TryGetComponent(out InteractiveObject dialoguesSo))
            {
                Debug.Log("No DialogueSystemSo found in this object or its parents");
                return;
            }
            
            _isInteracting = false;
            playerMovementManager.BlockWalk(true);
            playerMovementManager.BlockMovement(true);
            _isPaused = true;
            
            await dialoguesSo.interaction.Execute(_dialogueSystem, this);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in PlayerInteractionManager: {e.Message}");
        }
    }
    
    public void EndDialogue(DialogueSystemSo obj)
    {
        _dialogueSystem.OnComplicitEvent -= EndDialogue;
        EndInteraction();
    }
    
    public void OpenMap()
    {
        Debug.Log("OpenMap");
        _mapManager.OnCloseMap += EndMap;
        _mapManager.gameObject.SetActive(true);
    }

    private void EndMap()
    {
        _mapManager.OnCloseMap -= EndMap;
        EndInteraction();
    }
    
    private void EndInteraction()
    {
        _isPaused = false;
        playerMovementManager.BlockWalk(false);
        playerMovementManager.BlockMovement(false);
    }
    
    public void Interaction(InputAction.CallbackContext context)
    {
        if(context.started)_isInteracting = true;
        if(context.canceled)_isInteracting = false;
    }
    
    public void EditorInteractionMap(InputAction.CallbackContext context)
    {
        if(_roomEnterable is null) return;
        if(context.started) OnEditorInteractionEvent();
    }
    
    public void OnEditorInteractionEvent()
    {
        OnEditorInteraction?.Invoke();
    }

    public void SetRoomEnterable(RoomEnterable roomEnterable)
    {
        _roomEnterable = roomEnterable;
    }

    private void ChangeCamera()
    { 
        cameraInfo = CameraCenterTaverna.Instance.GetCamera();
        playerMovementManager.RoomEditor(cameraInfo);
    }
}
