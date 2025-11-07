using System;
using UnityEngine;

public class RoomEnterable : MonoBehaviour
{
    private bool _enableRoom;
    
    [SerializeField]
    private GameObject door;
    
    [SerializeField] private bool isDoorOpen;

    [SerializeField] private LayerMask playerLayer;

    private PlayerInteractionManager _playerInteractionManager;
    
    private bool isEditorOpen;
    
    private void OnEnable()
    {
        SetEnableRoom(isDoorOpen);
    }
    
    public event Action<RoomEnterable> OnRoomEntered;
    

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.layer.Equals(playerLayer)) return;
        
        PlayerEntered(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.layer.Equals(playerLayer)) return;
        
        PlayerExited(other);
    }
    
    private void PlayerEntered(Collider other)
    {
        if(_playerInteractionManager is not null) return;

        if (!other.TryGetComponent(out _playerInteractionManager))
        {
            return;
        }
        
        _playerInteractionManager.SetRoomEnterable(this);
        
        _playerInteractionManager.OnEditorInteraction += OpenEditor;
    }

    private void OpenEditor()
    {
        isEditorOpen =! isEditorOpen;

        if (isEditorOpen)
        {
            
            return;
        }
        
        //close editor
    }
    
    private void PlayerExited(Collider other)
    {
        if(_playerInteractionManager is null) return; 
        
        if(!_playerInteractionManager.gameObject.Equals(other.gameObject)) return;
        
        _playerInteractionManager.OnEditorInteraction -= OpenEditor;
        
        _playerInteractionManager.SetRoomEnterable(null);
        
        _playerInteractionManager = null;
    }
    
    #region Door
    public bool GetEnableRoom()
    {
        return _enableRoom;
    }
    
    public void SetEnableRoom(bool enableRoom)
    {
        if(_enableRoom == enableRoom) return;
        
        OnRoomEntered?.Invoke(this);
        _enableRoom = enableRoom;
        
        if(_enableRoom) OpenDoor();
        else CloseDoor();
    }
    
    private void OpenDoor()
    {
        door.SetActive(false);
    }

    private void CloseDoor()
    {
        door.SetActive(true);
    }
    #endregion
    
}
