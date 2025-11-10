using System;
using TMPro;
using UnityEngine;

public class RoomEnterable : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private bool isDoorOpen;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField]private GameObject editorUI;
    [SerializeField]private TMP_Text editorRoomText;
    [SerializeField,TextArea(0,1)]private string openEditorButton, closeEditorButton; 

    private bool _enableRoom;
    private PlayerInteractionManager _playerInteractionManager;
    private bool isEditorOpen;
    
    private void OnEnable()
    {
        SetEnableRoom(isDoorOpen);
    }
    
    public event Action<RoomEnterable> OnRoomEntered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"layer obj is {other.gameObject.layer} and layer for player is {playerLayer} and the competition is {IsInLayerMask(playerLayer, other.gameObject)}");
        if(!IsInLayerMask(playerLayer, other.gameObject)) return;
        
        PlayerEntered(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(!IsInLayerMask(playerLayer, other.gameObject)) return;
        
        PlayerExited(other);
    }
    
    private void PlayerEntered(Collider other)
    {
        if(_playerInteractionManager is not null) return;

        if (!other.TryGetComponent(out _playerInteractionManager))
        {
            return;
        }
        
        editorUI.SetActive(true);
        editorRoomText.text = openEditorButton;
        _playerInteractionManager.SetRoomEnterable(this);
        
        _playerInteractionManager.OnEditorInteraction += OpenEditor;
    }

    private void PlayerExited(Collider other)
    {
        if(_playerInteractionManager is null) return; 
        if(!_playerInteractionManager.gameObject.Equals(other.gameObject)) return;
        
        isEditorOpen = false;
        editorUI.SetActive(false);
        _playerInteractionManager.OnEditorInteraction -= OpenEditor;
        _playerInteractionManager.SetRoomEnterable(null);
        _playerInteractionManager = null;
    }
    
    private void OpenEditor()
    {
        isEditorOpen =! isEditorOpen;
        CameraCenterTaverna.Instance.ChangeCamera();
        
        if (isEditorOpen)
        {
            editorRoomText.text = closeEditorButton;
            return;
        }
        editorRoomText.text = openEditorButton;
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

    private static bool IsInLayerMask(LayerMask layerMask, GameObject gameObject) =>
        layerMask == (layerMask | (1 << gameObject.layer));
}