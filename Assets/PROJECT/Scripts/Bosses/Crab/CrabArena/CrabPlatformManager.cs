using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PathWay {
    public Vector3[] pathPoint;
}
public class CrabPlatformManager : MonoBehaviour {
    #region Parameters

    [Header("Atributes")]
    [SerializeField] CrabArenaSO arenaInfo;
    [SerializeField] GameObject platformObject;
    [SerializeField] List<PathWay> paths = new();
    [SerializeField] GameObject walls;
    [SerializeField] LayerMask platformOrAnimalLayer;
    bool _playerInPlatform;


    // Components
    ContinuosDamageHitBox _incomingTideAttack;
    GameObject _player;

    // Actions
    Action _onHandleHighTide, _onHandleLowTidde, _onHandleIncomingTide, _onHandleOutgoingTide;

    // Coroutine
    Coroutine _instantiateAnimalCoroutine;

    #endregion

    #region Initialize

    private void Awake() {

        _onHandleHighTide = HandleHighTide;
        _onHandleLowTidde = HandleLowTide;
        _onHandleIncomingTide = HandleIncomingTide;
        _onHandleOutgoingTide = HandleOutgoingTide;
    }

    private void Start() {
        CrabArenaManager.Instance.OnChangeToHighTide += _onHandleHighTide;
        CrabArenaManager.Instance.OnChangeToLowTide += _onHandleLowTidde;
        CrabArenaManager.Instance.OnChangeToIncomingTide += _onHandleIncomingTide;
        CrabArenaManager.Instance.OnChangeToOutgoingTide += _onHandleOutgoingTide;

        ArenaManager.Instance.SetPathPoints(paths);

        _player = PlayerManager.Instance.Player;
    }

    private void OnDestroy() {

        // UnSubscribe Events
        CrabArenaManager.Instance.OnChangeToHighTide -= _onHandleHighTide;
        CrabArenaManager.Instance.OnChangeToLowTide -= _onHandleLowTidde;
        CrabArenaManager.Instance.OnChangeToIncomingTide -= _onHandleIncomingTide;
        CrabArenaManager.Instance.OnChangeToOutgoingTide -= _onHandleOutgoingTide;

        // Kill DOTween
        platformObject.transform.DOKill();
    }
    #endregion

    #region HandleTides
    void HandleHighTide() {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformUpSpeed;

        ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Paths);

        if (walls != null) walls.SetActive(true);

        if (_player != null) _player.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformHighTideHeight, duration).OnComplete(() => {
            if (_player != null) _player.transform.SetParent(null);
        });

    }
    void HandleIncomingTide() {
        if (_playerInPlatform) {
            CrabArenaManager.Instance.ForceCurrentTideToEnd();
            return;
        }

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(arenaInfo.IncomingTideAttackHitBoxName,
            arenaInfo.IncomingTideAttackHitBox, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.position = Vector3.zero;
        hitbox.transform.localScale = Vector3.one * arenaInfo.IncomingTideAttackDamagSize;

        DamageContext context = new(
            arenaInfo.IncomingTideAttackDamage,
            arenaInfo.IncomingTideAttackDamage,
            arenaInfo.IncomingTideDuration - 1,
            false,
            DamageType.Pure,
            new() { Tags.Player, },
            CrabArenaManager.Instance.gameObject.GetComponent<StatusManager>(),
            new() {
                {ExtraDamageContextAtributes.DamageCooldown, arenaInfo.IncomingTideAttackDamageCooldown},
            }
            );

        _incomingTideAttack = hitbox.GetComponent<ContinuosDamageHitBox>();
        _incomingTideAttack.Initialize(context);

    }
    void HandleLowTide() {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformDownSpeed;

        if (_player != null) _player.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformLowTideHeight, duration).OnComplete(() => {
            if (_player != null) _player.transform.SetParent(null);
            if (walls != null) walls.SetActive(false);
        });
    }
    void HandleOutgoingTide() {
        ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Square);
        _instantiateAnimalCoroutine ??= StartCoroutine(InstantiateMarineAnimals());
    }

    IEnumerator InstantiateMarineAnimals() {

        for (int i = 0; i < arenaInfo.AmountOfAnimals; i++) {
            Debug.Log("Current Animal: " + i);
            bool foundAPlace = false;

            while (!foundAPlace) {
                Debug.Log("Tryin to find a place");

                Vector3 position = ArenaManager.Instance.GetRandomPosition();
                float floorHeight = ArenaManager.Instance.FindGroundHeight(position);
                Vector3 groundPosition = new(position.x, floorHeight, position.z);
                position.y += 100;

                Collider[] hitCollider = Physics.OverlapCapsule(groundPosition, position, arenaInfo.AnimalDistance, platformOrAnimalLayer);

                if(hitCollider.Length == 0) { // Não colidiu com a plataforma

                    int amountOfMaxAnimals = arenaInfo.ListOfAnimals.Count;
                    int rng = Random.Range(0, amountOfMaxAnimals);
                    var animal = arenaInfo.ListOfAnimals.ElementAt(rng);

                    GameObject prefab = animal.Value;
                    string prefabName = animal.Key;

                    GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefabName, prefab, TypeOfSkillPrefab.Hitbox);
                    hitbox.transform.position = groundPosition;
                    CrabMarineAnimal crabMarineAnimal = hitbox.GetComponent<CrabMarineAnimal>();

                    crabMarineAnimal.OnStart();

                    foundAPlace = true;
                }

                yield return null;
            }
        }

        _instantiateAnimalCoroutine = null;
    }

    #endregion

    #region Trigger
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Colisao");

        _playerInPlatform = true;

        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.IncomingTide) {
            _incomingTideAttack.End();
            CrabArenaManager.Instance.ForceCurrentTideToEnd();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player") || !_playerInPlatform) return;

        _playerInPlatform = false;
    }
    #endregion
}
