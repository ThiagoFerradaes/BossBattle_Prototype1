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
        Player.GetComponent<HealthManager>().OnDeath += Defeat;
    }

    private void OnDestroy() {
        Player.GetComponent<HealthManager>().OnDeath -= Defeat;
    }
    void Defeat() {
        ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Defeat);
    }
}
