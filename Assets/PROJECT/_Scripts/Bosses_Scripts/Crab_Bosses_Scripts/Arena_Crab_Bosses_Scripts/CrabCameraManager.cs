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
        CrabArenaManager.Instance.OnStartTide += HandleChangesOfTide;
    }
    private void OnDestroy() {
        CrabArenaManager.Instance.OnStartTide -= HandleChangesOfTide;
    }
    #endregion

    #region Handle Cameras
    void HandleChangesOfTide(CrabArenaState state)
    {
        if (state == CrabArenaState.LowTide) HandleLowTide();
        else if (state == CrabArenaState.HighTide) HandleHighTide();
    }
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
