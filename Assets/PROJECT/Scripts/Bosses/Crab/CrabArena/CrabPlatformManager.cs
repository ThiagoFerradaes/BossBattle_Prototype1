using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PathWay
{
    public List<Transform> pathPoint;
}
public class CrabPlatformManager : MonoBehaviour
{
    #region Parameters

    [Header("Atributes")]
    [SerializeField] CrabArenaSO arenaInfo;
    [SerializeField] GameObject platformObject;
    [SerializeField] List<PathWay> paths = new();
    [SerializeField] List<GameObject> walls;
    [SerializeField] List<GameObject> platformWarningObject;
    [SerializeField] List<InstantDamageHitBox> platformDamageColliderObject;
    [SerializeField] LayerMask platformOrAnimalLayer;
    int _platformContacts = 0;

    [Header("High Tide Barriers")]
    [SerializeField] List<Transform> listOfPossibleBarriers;
    Dictionary<Transform, GameObject> _dictionaryOfBombs = new();

    // Components
    ContinuosDamageHitBox _incomingTideAttack;
    GameObject _player;

    // Actions
    Action<CrabArenaState> _onHandleChangeTide;

    // Coroutine
    Coroutine _instantiateAnimalCoroutine;
    Coroutine _highTideBombCoroutine;

    #endregion

    #region Initialize

    private void Awake()
    {

        _onHandleChangeTide = HandleChangeTide;
    }

    private void Start()
    {
        CrabArenaManager.Instance.OnStartTide += _onHandleChangeTide;

        ArenaManager.Instance.SetPathPoints(paths);

        _player = PlayerManager.Instance.Player;

        foreach (var path in paths)
        {
            foreach (var t in path.pathPoint)
            {
                if (_dictionaryOfBombs.ContainsKey(t)) continue;

                _dictionaryOfBombs[t] = Instantiate(arenaInfo.BombPrefab);
                _dictionaryOfBombs[t].transform.localScale = Vector3.one * arenaInfo.BombSize;
            }
        }
    }

    private void OnDestroy()
    {

        // UnSubscribe Events
        CrabArenaManager.Instance.OnStartTide -= _onHandleChangeTide;

        // Kill DOTween
        platformObject.transform.DOKill();
    }
    #endregion

    #region HandleTides
    void HandleChangeTide(CrabArenaState state)
    {
        switch (state)
        {
            case CrabArenaState.LowTide:
                HandleLowTide();
                break;
            case CrabArenaState.IncomingTide:
                HandleIncomingTide();
                break;
            case CrabArenaState.HighTide:
                HandleHighTide();
                break;
            case CrabArenaState.OutgoingTide:
                HandleOutgoingTide();
                break;
        }
    }
    void HandleHighTide()
    {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformUpSpeed;

        ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Paths);

        if (walls != null) foreach (var wall in walls) wall.SetActive(true);

        if (_player != null) _player.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformHighTideHeight, duration).OnComplete(() =>
        {
            if (_player != null) _player.transform.SetParent(null);
        });

        _highTideBombCoroutine ??= StartCoroutine(HandleHighTideBombs());
    }
    void HandleIncomingTide()
    {
        if (_platformContacts > 0)
        {
            CrabArenaManager.Instance.ChangeCurrentTide();
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
    void HandleLowTide()
    {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformDownSpeed;

        if (_player != null) _player.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformLowTideHeight, duration).OnComplete(() =>
        {
            if (_player != null) _player.transform.SetParent(null);
            if (walls != null) foreach (var wall in walls) wall.SetActive(false);
        });
    }
    void HandleOutgoingTide()
    {
        ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Square);
        _instantiateAnimalCoroutine ??= StartCoroutine(InstantiateMarineAnimals());
    }

    IEnumerator InstantiateMarineAnimals()
    {

        for (int i = 0; i < arenaInfo.AmountOfAnimals; i++)
        {
            bool foundAPlace = false;

            while (!foundAPlace)
            {

                Vector3 position = ArenaManager.Instance.GetRandomPosition();
                float floorHeight = ArenaManager.Instance.FindGroundHeight(position);
                Vector3 groundPosition = new(position.x, floorHeight, position.z);

                Collider[] hitCollider = Physics.OverlapSphere(groundPosition, arenaInfo.AnimalDistance, platformOrAnimalLayer);

                if (hitCollider.Length == 0)
                { // Não colidiu com a plataforma

                    int amountOfMaxAnimals = arenaInfo.ListOfAnimals.Count;
                    int rng = Random.Range(0, amountOfMaxAnimals);
                    var animal = arenaInfo.ListOfAnimals.ElementAt(rng);

                    GameObject prefab = animal.Value;
                    string prefabName = animal.Key;

                    GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefabName, prefab, TypeOfSkillPrefab.Hitbox);
                    groundPosition.y = arenaInfo.AnimalHeight;
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

    IEnumerator HandleHighTideBombs()
    {
        while (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide)
        {
            yield return new WaitForSeconds(arenaInfo.BombsCooldownToAppear);

            bool allBombsActivated = true;
            foreach (var bomb in _dictionaryOfBombs.Values)
            {
                if (bomb.activeInHierarchy == false) allBombsActivated = false;
            }

            if (allBombsActivated) continue;


            int rngPaths = Random.Range(0, paths.Count);

            bool allBombsInPathAreActivated = true;

            while (allBombsInPathAreActivated)
            {
                foreach (Transform t in paths[rngPaths].pathPoint)
                {
                    if (_dictionaryOfBombs[t].activeInHierarchy == false) allBombsInPathAreActivated = false;
                }

                rngPaths = (rngPaths + 1) % paths.Count;

                yield return null;
            }

            int rngBomb = Random.Range(0, paths[rngPaths].pathPoint.Count);

            bool bombActivated = true;

            while (bombActivated)
            {
                if (_dictionaryOfBombs[paths[rngPaths].pathPoint[rngBomb]].activeInHierarchy == false) bombActivated = false;

                rngBomb = (rngBomb + 1) % paths[rngPaths].pathPoint.Count;

                yield return null;
            }

            Vector3 bombPos = paths[rngPaths].pathPoint[rngBomb].position;
            bombPos.y += arenaInfo.BombHeight;
            _dictionaryOfBombs[paths[rngPaths].pathPoint[rngBomb]].transform.position = bombPos;
            _dictionaryOfBombs[paths[rngPaths].pathPoint[rngBomb]].SetActive(true);
        }

        _highTideBombCoroutine = null;
    }
    #endregion

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _platformContacts++;
        if (_platformContacts == 1)
        {
            if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.IncomingTide)
            {
                _incomingTideAttack.End();
                CrabArenaManager.Instance.ChangeCurrentTide();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _platformContacts--;
        if (_platformContacts <= 0)
        {
            _platformContacts = 0;
        }
    }
    #endregion

    #region Getter

    public List<GameObject> ReturnPlatformWarningObject() => platformWarningObject;
    public List<InstantDamageHitBox> ReturnPlatformDamageCollider() => platformDamageColliderObject;

    #endregion
}
