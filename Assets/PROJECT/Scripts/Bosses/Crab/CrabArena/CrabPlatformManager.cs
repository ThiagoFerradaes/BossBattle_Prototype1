using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathWay {
    public Vector3[] pathPoint;
}
public class CrabPlatformManager : MonoBehaviour {
    #region Parameters

    [Header("Atributes")]
    [SerializeField] ArenaCrabSO arenaInfo;
    [SerializeField] GameObject platformObject;
    [SerializeField] List<PathWay> paths = new();
    [SerializeField] GameObject walls;
    bool _playerInPlatform;

    // Components
    ContinuosDamageHitBox _incomingTideAttack;
    GameObject _player;

    // Actions
    Action _onHandleHighTide, _onHandleLowTidde, _onHandleIncomingTide;

    #endregion

    #region Initialize

    private void Awake() {

        _onHandleHighTide = HandleHighTide;
        _onHandleLowTidde = HandleLowTide;
        _onHandleIncomingTide = HandleIncomingTide;
    }

    private void Start() {
        ArenaCrabManager.Instance.OnChangeToHighTide += _onHandleHighTide;
        ArenaCrabManager.Instance.OnChangeToLowTide += _onHandleLowTidde;
        ArenaCrabManager.Instance.OnChangeToIncomingTide += _onHandleIncomingTide;

        ArenaManager.Instance.SetPathPoints(paths);

        _player = PlayerManager.Instance.Player;
    }

    private void OnDestroy() {

        // UnSubscribe Events
        ArenaCrabManager.Instance.OnChangeToHighTide -= _onHandleHighTide;
        ArenaCrabManager.Instance.OnChangeToLowTide -= _onHandleLowTidde;
        ArenaCrabManager.Instance.OnChangeToIncomingTide -= _onHandleIncomingTide;

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
            ArenaCrabManager.Instance.ForceCurrentTideToEnd();
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
            ArenaCrabManager.Instance.gameObject.GetComponent<StatusManager>(),
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
            ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Square);
            if (walls != null) walls.SetActive(false);
        });
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Colisao");

        _playerInPlatform = true;

        if (ArenaCrabManager.Instance.ReturnCurrentTide() == CrabArenaState.IncomingTide) {
            _incomingTideAttack.End();
            ArenaCrabManager.Instance.ForceCurrentTideToEnd();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player") || !_playerInPlatform) return;

        _playerInPlatform = false;
    }
    #endregion
}
