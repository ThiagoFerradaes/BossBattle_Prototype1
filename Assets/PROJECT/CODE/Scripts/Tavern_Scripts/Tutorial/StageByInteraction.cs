using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageByInteraction : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;

    [SerializeField]private LayerMask playerLayer;

    [SerializeField]private float interactionRange = 1f;

    private PlayerActionMap playerActionMap;

    private Transform player;
    
    private void OnEnable()
    {
        playerActionMap = new PlayerActionMap();
        playerActionMap.Player.Interaction.started += Interaction;
        playerActionMap.Enable();
        player = PlayerManager.Instance.Player.transform;
    }

    private void OnDisable()
    {
        playerActionMap.Player.Interaction.started -= Interaction;
        playerActionMap.Disable();
    }


    public void Interaction(InputAction.CallbackContext context)
    {
        if (Vector3.Distance(player.position, transform.position) > interactionRange) return;
        
        OnCompleteTutorialEvent ?.Invoke(true);
    }
}
