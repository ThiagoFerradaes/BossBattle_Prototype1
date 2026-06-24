using UnityEngine;

public class RegularObjectToInteract : MonoBehaviour, IInteractable
{
    [SerializeField] RegularObjectSO regularObjectSO;
    public void Interact(PlayerInputHandlerManager handler)
    {
        RegularObjectUIManager.Instance.InitializeInteractionScreen(regularObjectSO.objectLine.GetLocalizedString());
    }
}

