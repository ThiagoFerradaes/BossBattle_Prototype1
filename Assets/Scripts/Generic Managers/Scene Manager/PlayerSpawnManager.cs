using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    public GameObject Player;
    public Transform CameraCenter;
    public CinemachineCamera MainCamera;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}
