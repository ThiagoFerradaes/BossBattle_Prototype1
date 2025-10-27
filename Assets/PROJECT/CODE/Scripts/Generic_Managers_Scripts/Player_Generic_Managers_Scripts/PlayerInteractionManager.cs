using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    private PlayerActionMap _playerInput;

    private bool _inputActionsEnabled;
    
    private bool _isInteracting;
    
    private bool _isPaused;
    
    [SerializeField, Tooltip("The player's interaction range")]
    private float interactionRange;

    private DialogueSystem _dialogueSystem;
    
    [SerializeField, Tooltip("The player's interaction LayerMask")]
    private LayerMask interactionLayer;
    
    [SerializeField,Tooltip("PlayerMovementManager reference")]
    private PlayerMovementManager playerMovementManager;
    
    private void OnEnable()
    {
        _playerInput = new PlayerActionMap();
        _playerInput.Enable();
        _dialogueSystem = FindAnyObjectByType<DialogueSystem>();
        try
        {
            _playerInput.Player.Interaction.started += StartInteraction;
            _playerInput.Player.Interaction.canceled += EndInteraction;

            _inputActionsEnabled = true;
        }
        catch
        {
            // Fallback if InputActions setup fails
            _inputActionsEnabled = false;
        }
    }

    private void OnDisable()
    {
        if (!_inputActionsEnabled) return;
        
        _playerInput.Player.Interaction.started -= StartInteraction;
        _playerInput.Player.Interaction.canceled -= EndInteraction;
        _playerInput.Disable();
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

            InteractiveObjectForDialogue dialoguesSo;
            if (!hit.collider.gameObject.TryGetComponent(out dialoguesSo))
            {
                Debug.Log("No DialogueSystemSo found in this object or its parents");
                return;
            }
            

            _isInteracting = false;
            _playerInput.Disable();
            playerMovementManager.BlockWalk(true);
            playerMovementManager.BlockMovement(true);
            _isPaused = true;
            
            _dialogueSystem.gameObject.SetActive(true);
            
            _dialogueSystem.OnComplicitEvent += EndInteraction;
            await _dialogueSystem.NewDialogue(dialoguesSo.dialogue);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in PlayerInteractionManager: {e.Message}");
        }
    }
    
    private void EndInteraction(DialogueSystemSo obj)
    {
        _dialogueSystem.OnComplicitEvent -= EndInteraction;
        _isPaused = false;
        _playerInput.Enable();
        playerMovementManager.BlockWalk(false);
        playerMovementManager.BlockMovement(false);
    }

    private void StartInteraction(InputAction.CallbackContext context)
    {
        _isInteracting = true;
    }
    
    private void EndInteraction(InputAction.CallbackContext context)
    {
        _isInteracting = false;
    }
    
}
