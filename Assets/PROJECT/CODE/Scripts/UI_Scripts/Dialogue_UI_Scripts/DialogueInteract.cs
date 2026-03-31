using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] Dialogue dialogue;

    public void Interact(PlayerInputHandlerManager handler)
    {
        DialogueManager.Instance.InitializeDialogue(dialogue.RootNode);
    }
}
