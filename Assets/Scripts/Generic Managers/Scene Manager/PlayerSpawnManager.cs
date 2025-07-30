using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    public GameObject Player;
    public Transform CameraCenter;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}
