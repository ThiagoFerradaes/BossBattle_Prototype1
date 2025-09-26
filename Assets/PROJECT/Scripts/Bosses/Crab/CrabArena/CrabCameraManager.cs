using System;
using Unity.Cinemachine;
using UnityEngine;

public class CrabCameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera lowTideCamera;
    [SerializeField] CinemachineCamera highTideCamera;

    Action _onHandleHighTide, _onHandleLowTide;

    #region Initialize
    private void Awake() {
        _onHandleHighTide = HandleHighTide;
        _onHandleLowTide = HandleLowTide;

        HandleLowTide();
    }
    private void Start() {
        CrabArenaManager.Instance.OnChangeToHighTide += _onHandleHighTide;
        CrabArenaManager.Instance.OnChangeToLowTide += _onHandleLowTide;
    }
    private void OnDestroy() {
        CrabArenaManager.Instance.OnChangeToHighTide -= _onHandleHighTide;
        CrabArenaManager.Instance.OnChangeToLowTide -= _onHandleLowTide;
    }
    #endregion

    #region Handle Cameras
    void HandleHighTide() {
        highTideCamera.Priority = 1;
        lowTideCamera.Priority = 0;
    }

    void HandleLowTide() {
        highTideCamera.Priority = 0;
        lowTideCamera.Priority = 1;
    }
    #endregion
}
