using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    public GameObject Player;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}
