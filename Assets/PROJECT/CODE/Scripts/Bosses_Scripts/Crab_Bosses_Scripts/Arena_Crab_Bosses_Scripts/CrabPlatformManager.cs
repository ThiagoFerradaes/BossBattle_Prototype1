using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializedDictionary("Wall", "Transforms"), SerializeField] SerializedDictionary<CrabArenaWall, List<Transform>> listOfPossibleBarriers;
    GameObject _barrier;
    Dictionary<Transform, GameObject> _dictionaryOfBombs = new();

    // Components
    ContinuosDamageHitBox _incomingTideAttack;
    GameObject _player;
    CrabManager _crabManager;

    // Actions
    Action<CrabArenaState> _onHandleChangeTide;

    // Coroutine
    Coroutine _instantiateAnimalCoroutine;
    Coroutine _highTideBombCoroutine;
    Coroutine _highTideBarrierCoroutine;

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

        _crabManager = CrabArenaManager.Instance.CrabM;

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

        _barrier = Instantiate(arenaInfo.BarrierPrefab);
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
        _highTideBarrierCoroutine ??= StartCoroutine(HandleHidhTideBarrier());
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
            arenaInfo.IncomingTideDamageAtributes,
            CrabArenaManager.Instance.gameObject.GetComponent<StatusManager>()
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
        yield return new WaitForSeconds(arenaInfo.BombsCooldownToAppear/3);

        while (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide)
        {
            var inactiveBombs = _dictionaryOfBombs
                .Where(kv => !kv.Value.activeInHierarchy)
                .ToList();

            if (inactiveBombs.Count == 0)
                continue; 

            var chosenBomb = inactiveBombs[Random.Range(0, inactiveBombs.Count)];
            Transform point = chosenBomb.Key;

            Vector3 bombPos = point.position;
            bombPos.y += arenaInfo.BombHeight;

            var bombGO = chosenBomb.Value;
            bombGO.transform.position = bombPos;
            bombGO.GetComponent<HealthManager>().Revive();
            bombGO.SetActive(true);

            float timer = 0;
            while (timer < arenaInfo.BombsCooldownToAppear && CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        foreach (var bomb in _dictionaryOfBombs.Values)
        {
            bomb.SetActive(false);
        }

        _highTideBombCoroutine = null;
    }

    IEnumerator HandleHidhTideBarrier()
    {
        yield return new WaitForSeconds(arenaInfo.TimeToSpawnBarrier);

        _barrier.GetComponent<HealthManager>().Revive();

        CrabArenaWall _currentWall = _crabManager.ReturnCurrentWall();

        int rng = Random.Range(0, listOfPossibleBarriers[_currentWall].Count);

        Vector3 pos = listOfPossibleBarriers[_currentWall][rng].position;
        float finalHeight = pos.y;
        pos.y -= arenaInfo.BarrierDownOffset;

        _barrier.transform.position = pos;
        _barrier.transform.rotation = _currentWall == CrabArenaWall.Up ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
        _barrier.SetActive(true);

        yield return _barrier.transform.DOMoveY(finalHeight, arenaInfo.BarrierUpDuration);

        _highTideBarrierCoroutine = null;
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
