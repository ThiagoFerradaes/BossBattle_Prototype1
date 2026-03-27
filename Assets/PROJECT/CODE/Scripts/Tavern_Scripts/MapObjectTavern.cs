using UnityEngine;

public class MapObjectTavern : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject mapCanvas;
    public void Interact()
    {
        mapCanvas.SetActive(true);
    }
}
