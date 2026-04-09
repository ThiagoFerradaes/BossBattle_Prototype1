using UnityEngine;

public class RegularObjectToInteract : MonoBehaviour, IInteractable
{
    [SerializeField] RegularObjectSO regularObjectSO;
    public void Interact(PlayerInputHandlerManager handler)
    {
        RegularObjectUIManager.Instance.InitializeScreen(regularObjectSO.objectLine.GetLocalizedString());
    }
}

