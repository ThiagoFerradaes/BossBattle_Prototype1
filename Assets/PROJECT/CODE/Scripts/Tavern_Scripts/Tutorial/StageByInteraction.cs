using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageByInteraction : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;

    [SerializeField]private LayerMask playerLayer;

    [SerializeField]private float interactionRange = 1f;
    
    public void Interaction(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        if(Physics.SphereCast(transform.position, interactionRange, Vector3.down, out RaycastHit hit, interactionRange, playerLayer))
        {
            OnCompleteTutorialEvent ?.Invoke(true);
        }

    }
}
