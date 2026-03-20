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
    [HideInInspector] public GameObject Player;
    [SerializeField] bool isTavernScene = false;
    [ShowIf("isTavernScene"), AllowNesting, SerializeField] GameObject julianPrefab;

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

        GameObject player;

        switch (isTavernScene)
        {
            case true:
                player = Instantiate(julianPrefab, PlayerSpawnPoint.position, Quaternion.identity);
                break;

            case false:
                CharacterSO currentCharacter = _playerWhiteBoard.ReturnSelectedCharacterSO();
                player = Instantiate(currentCharacter.CharacterPrefab, PlayerSpawnPoint.position, Quaternion.identity);
                break;
        }

        Player = player;

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

    public void SetPlayer(GameObject newPlayer) {
        Player = newPlayer;
    }
}
