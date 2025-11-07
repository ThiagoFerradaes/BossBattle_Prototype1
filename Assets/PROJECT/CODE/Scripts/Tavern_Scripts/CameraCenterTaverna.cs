using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCenterTaverna : MonoBehaviour
{
    public static CameraCenterTaverna Instance;

    [SerializeField] private CinemachineCamera cinemaChineCameraPersona, cinemaChineCameraEditor;
    
    public event Action OnCameraChanged;
    
    private Transform _playerTransform;
    
    private bool cameraIsPersona = true;
    
    [Header("Input Action Reference")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private float cameraSpeed;
    
    private float _distanceForCameraByPerson;
    private float _distanceForCameraByEditor;
    
    private float _distanceForCameraForY;
    public void Awake()
    {
        if (Instance is not null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _distanceForCameraByPerson = cinemaChineCameraPersona.gameObject.transform.position.z - transform.position.z; 
        _distanceForCameraByEditor = cinemaChineCameraEditor.gameObject.transform.position.z - transform.position.z; 
        _distanceForCameraForY = cinemaChineCameraPersona.gameObject.transform.position.y - transform.position.y;
    }

    public bool GetCamera()
    {
        return cinemaChineCameraPersona.Priority == 1;
    }
    
    public void ChangeCamera()
    {
        OnCameraChanged?.Invoke();
        
        if (cinemaChineCameraPersona.Priority == 0)
        {
            cameraIsPersona = true;
            cinemaChineCameraPersona.Priority = 1;
            cinemaChineCameraEditor.Priority = 0;
            return;
        }

        cinemaChineCameraEditor.gameObject.transform.position = new Vector3(_playerTransform.position.x, _distanceForCameraForY, _playerTransform.position.z+_distanceForCameraByEditor);
        
        cameraIsPersona = false;
        cinemaChineCameraPersona.Priority = 0;
        cinemaChineCameraEditor.Priority = 1;
    }
    
    public void SetPlayerTransform(Transform playerTransform) => _playerTransform = playerTransform;

    private void Update()
    {
        if (_playerTransform is null) return;
        
        if (cameraIsPersona)
        {
            cinemaChineCameraPersona.gameObject.transform.position = new Vector3(_playerTransform.position.x, _distanceForCameraForY, _playerTransform.position.z+_distanceForCameraByPerson);
            return;
        }
        
        var value = moveActionReference.action.ReadValue<Vector2>();
        value *= cameraSpeed * Time.deltaTime;

        cinemaChineCameraEditor.gameObject.transform.position = new Vector3(value.x+cinemaChineCameraEditor.gameObject.transform.position.x, _distanceForCameraForY, value.y+cinemaChineCameraEditor.gameObject.transform.position.z);
    }
}
