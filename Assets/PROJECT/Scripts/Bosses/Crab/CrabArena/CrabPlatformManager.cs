using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class CrabPlatformManager : MonoBehaviour
{
    #region Parameters

    [SerializeField] ArenaCrabSO arenaInfo;
    [SerializeField] GameObject platformObject;
    [SerializeField] GameObject palayer;

    // Actions
    Action _onHandleHighTide, _onHandleLowTidde;

    // Coroutines
    Coroutine _onMovePlatformCoroutine;
    #endregion

    #region Initialize

    private void Awake() {

        _onHandleHighTide = HandleHighTide;
        _onHandleLowTidde = HandleLowTide;

    }

    private void Start() {
        ArenaCrabManager.Instance.OnChangeToHighTide += _onHandleHighTide;
        ArenaCrabManager.Instance.OnChangeToLowTide += _onHandleLowTidde;
    }

    private void OnDestroy() {

        // UnSubscribe Events
        ArenaCrabManager.Instance.OnChangeToHighTide -= _onHandleHighTide;
        ArenaCrabManager.Instance.OnChangeToLowTide -= _onHandleLowTidde;

        // Kill DOTween
        platformObject.transform.DOKill();
    }
    #endregion

    void HandleHighTide() {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformUpSpeed;

        palayer.transform.SetParent(platformObject.transform);
        platformObject.transform.DOMoveY(arenaInfo.PlatformHighTideHeight, duration).OnComplete(() => {
            palayer.transform.SetParent(null);
        });

    }

    void HandleLowTide() {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformDownSpeed;

        palayer.transform.SetParent(platformObject.transform);
        platformObject.transform.DOMoveY(arenaInfo.PlatformLowTideHeight, duration).OnComplete(() => {
            palayer.transform.SetParent(null);
        });
    }

}
