using Unity.Cinemachine;
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

    private void Start() {
        Player.GetComponent<HealthManager>().OnDeath -= Defeat;
        Player.GetComponent<HealthManager>().OnDeath += Defeat;
    }

    private void OnDisable() {
        if (Player != null) {
            if (Player.TryGetComponent<HealthManager>(out var health))
                health.OnDeath -= Defeat;
        }
    }

    void Defeat() {
        ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Defeat);
    }
}
