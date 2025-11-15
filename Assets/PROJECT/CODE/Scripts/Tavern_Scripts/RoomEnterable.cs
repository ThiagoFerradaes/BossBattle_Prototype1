using System;
using MyEnum;
using TMPro;
using UnityEngine;

public class RoomEnterable : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private bool isDoorOpen;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField]private GameObject editorUI;
    [SerializeField]private TMP_Text editorRoomText;
    [SerializeField] private TextBoxesSo openEditorButtonText, closeEditorButtonText;
    private string openEditorButton, closeEditorButton; 

    private bool _enableRoom;
    private PlayerInteractionManager _playerInteractionManager;
    private bool isEditorOpen;
    private ConfigurationSo _config;
    
    
    #region Unity Lifecycle Methods
    
    private void OnEnable()
    {
        SetEnableRoom(isDoorOpen);
        InitializeConfiguration();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Lang
    private void InitializeConfiguration()
    {
        _config = Resources.Load<ConfigurationSo>("Configuration/ConfigurationSO");

        if (_config == null) return;

        _config.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage(_config.GetLanguage());
    }
    
    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }
    
    private void UpdateLanguage(EnumLanguage lang)
    {
        openEditorButton = openEditorButtonText.GetText(lang);
        closeEditorButton = closeEditorButtonText.GetText(lang);
    }

    #endregion
    
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