using System;
using UnityEngine;

public class RoomEnterable : MonoBehaviour
{
    private bool _enableRoom;
    
    [SerializeField]
    private GameObject door;
    
    [SerializeField] private bool isDoorOpen;

    private void OnEnable()
    {
        SetEnableRoom(isDoorOpen);
    }
    
    public event Action<RoomEnterable> OnRoomEntered;

    
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
