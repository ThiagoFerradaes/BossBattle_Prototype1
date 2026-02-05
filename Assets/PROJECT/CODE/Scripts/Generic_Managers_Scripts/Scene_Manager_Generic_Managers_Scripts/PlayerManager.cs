using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using Unity.Cinemachine;
using UnityEngine;

public enum Character { Cyrus, Bastian, Lilian, Gracia, TavernKeeper, Null }
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

    Action _onDefeat;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _playerWhiteBoard = CurrentSelectedCharacterWhiteBoard.Instance;

        _onDefeat = Defeat;

        SpawnPlayer();
    }

    void SpawnPlayer() {
        if (_playerWhiteBoard == null) return;

        if (isTavernScene) {
            GameObject player = Instantiate(characterPrefabDictionary[Character.TavernKeeper], PlayerSpawnPoint.position, Quaternion.identity);
            Player = player;
            return;
        }

        Character currentCharacter = _playerWhiteBoard.ReturnSelectedCharacter();

        if (characterPrefabDictionary.ContainsKey(currentCharacter)) {
            GameObject player = Instantiate(characterPrefabDictionary[currentCharacter], PlayerSpawnPoint.position, Quaternion.identity);
            Player = player;
        }

    }
    private void Start() {
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
