using UnityEngine;

public class MapObjectTavern : MonoBehaviour, IInteractable
{
    [SerializeField] MapManager mapCanvas;
    public void Interact(PlayerInputHandlerManager handler)
    {
        handler.SetCanInput(false);
        mapCanvas.InitializeMap(handler);
    }
}
