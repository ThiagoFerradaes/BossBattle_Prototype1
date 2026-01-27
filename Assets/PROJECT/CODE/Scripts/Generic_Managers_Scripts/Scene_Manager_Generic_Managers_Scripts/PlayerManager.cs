using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using Unity.Cinemachine;
using UnityEngine;

public enum Character { Cyrus, Bastian, Lilian, Shapeshifter, SamuraiFrog, Sequencer, TavernKeeper, Null }
public class PlayerManager : MonoBehaviour {
    // Singleton
    public static PlayerManager Instance;

    public Transform CameraCenter;
    public Transform PlayerSpawnPoint;
    [Foldout("Dictionary"), SerializedDictionary("Character", "PraFab"), SerializeField]
    SerializedDictionary<Character, GameObject> characterPrefabDictionary = new();
    [HideInInspector] public GameObject Player;
    [SerializeField] bool isTavernScene = false;

    CurrentSelectedCharacterWhiteBoard _playerWhiteBoard;
    PoolingManager _pooling;

    Action _onDefeat;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _playerWhiteBoard = CurrentSelectedCharacterWhiteBoard.Instance;
        _pooling = GetComponent<PoolingManager>();

        _onDefeat = Defeat;

        SpawnPlayer();
    }

    public void SpawnPlayer(Vector3? playerPosition = null, bool initialize = false) {
        if (_playerWhiteBoard == null ) return;

        if (_pooling == null) Debug.Log("No pooling");

        Vector3 position = playerPosition.HasValue ? playerPosition.Value : PlayerSpawnPoint.position;

        if (isTavernScene) {
            GameObject player = Instantiate(characterPrefabDictionary[Character.TavernKeeper], position, Quaternion.identity);
            Player = player;
            return;
        }

        Character currentCharacter = _playerWhiteBoard.ReturnSelectedCharacter();

        if (characterPrefabDictionary.ContainsKey(currentCharacter)) {
            GameObject player = _pooling.ReturnCharacterObjectFromPool(characterPrefabDictionary[currentCharacter]);
            player.transform.SetPositionAndRotation(position, Quaternion.identity);
            player.SetActive(true);
            Player = player;         
        }

        if (initialize) InitializeCurrentPlayer();
    }

    public void InitializeCurrentPlayer() {

        Player.GetComponent<PlayerSkillCooldownManager>().Initialize();
        Player.GetComponent<EnergyManager>().Initialize(Player);
        Player.GetComponent<PlayerMovementManager>().Initialize();
        Player.GetComponent<PlayerSkillManager>().Initialize();
    }
    private void Start() {

        if (!isTavernScene) InitializeCurrentPlayer();

        if (Player != null && Player.TryGetComponent<HealthManager>(out HealthManager healthManager)) {
            healthManager.OnDeath += _onDefeat;
        }

    }

    private void OnDisable() {
        if (Player != null && Player.TryGetComponent<HealthManager>(out var health)) {
            health.OnDeath -= _onDefeat;
        }
    }

    void Defeat() {
        ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Defeat);
    }

    public void SetPlayer(GameObject newPlayer)
    {
        Player = newPlayer;
    }
}
