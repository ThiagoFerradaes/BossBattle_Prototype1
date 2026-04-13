using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Dialogue dialogue;

    public void Interact(PlayerInputHandlerManager handler)
    {
        handler.SetCanInput(false);
        DialogueManager.Instance.InitializeDialogue(dialogue.RootNode, handler);
    }
}
