using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public enum Character { WeaponMaster, Shooter, Summoner, Shapeshifter, SamuraiFrog, Sequencer}
public class PlayerManager : MonoBehaviour
{
    // Singleton
    public static PlayerManager Instance;

    public Transform CameraCenter;
    public Transform PlayerSpawnPoint;
    [Foldout("Dictionary"), SerializedDictionary("Character", "PraFab"), SerializeField]
    SerializedDictionary<Character, GameObject> characterPrefabDictionary = new();
    [HideInInspector] public GameObject Player;

    PlayerWhiteBoard _playerWhiteBoard;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _playerWhiteBoard = PlayerWhiteBoard.Instance;

        SpawnPlayer();
    }

    void SpawnPlayer() {
        if (_playerWhiteBoard == null) return;

        Character currentCharacter = _playerWhiteBoard.ReturnSelectedCharacter();

        if (characterPrefabDictionary.ContainsKey(currentCharacter)) {
            GameObject player = Instantiate(characterPrefabDictionary[currentCharacter], PlayerSpawnPoint.position, Quaternion.identity);
            Player = player;
        }

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
