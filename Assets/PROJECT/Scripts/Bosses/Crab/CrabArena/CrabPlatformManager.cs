using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathWay {
    public Vector3[] pathPoint;
}
public class CrabPlatformManager : MonoBehaviour
{
    #region Parameters

    [SerializeField] ArenaCrabSO arenaInfo;
    [SerializeField] GameObject platformObject;
    [SerializeField] List<PathWay> paths = new();


    // Actions
    Action _onHandleHighTide, _onHandleLowTidde;

    #endregion

    #region Initialize

    private void Awake() {

        _onHandleHighTide = HandleHighTide;
        _onHandleLowTidde = HandleLowTide;
    }

    private void Start() {
        ArenaCrabManager.Instance.OnChangeToHighTide += _onHandleHighTide;
        ArenaCrabManager.Instance.OnChangeToLowTide += _onHandleLowTidde;

        ArenaManager.Instance.SetPathPoints(paths);
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

        ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Paths);

        //palayer.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformHighTideHeight, duration).OnComplete(() => {
            //palayer.transform.SetParent(null);
        });

    }

    void HandleLowTide() {
        float deltaDistance = arenaInfo.PlatformHighTideHeight - arenaInfo.PlatformLowTideHeight;
        float duration = deltaDistance / arenaInfo.PlatformDownSpeed;

        //palayer.transform.SetParent(platformObject.transform);
        platformObject.transform.DOLocalMoveY(arenaInfo.PlatformLowTideHeight, duration).OnComplete(() => {
            //palayer.transform.SetParent(null);
            ArenaManager.Instance.SetTypeOfArena(TypeOfArena.Square);
        });
    }

}
