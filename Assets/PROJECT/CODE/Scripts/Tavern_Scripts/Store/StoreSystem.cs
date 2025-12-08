using UnityEngine;

public class StoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject storeUI;
    
    private PlayerInteractionManager _playerInteractionManager;
    
    public void OpenStore(PlayerInteractionManager playerInteractionManager) 
    {
        _playerInteractionManager = playerInteractionManager;
        storeUI.SetActive(true);
    }
    
    public void CloseStore() 
    {
        _playerInteractionManager.EndInteraction();
        storeUI.SetActive(false);
    }
}
