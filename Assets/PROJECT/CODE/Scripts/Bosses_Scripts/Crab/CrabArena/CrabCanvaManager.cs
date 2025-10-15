using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrabCanvaManager : MonoBehaviour
{
    [SerializeField] Image tidesBar;
    [SerializeField] TextMeshProUGUI tidesTitle;
    [SerializeField] string lowTideName;
    [SerializeField] string highTideName;

    CrabArenaState _currentTide;

    Action<float, float> _onUpdateTideBar;
    Action<CrabArenaState> _onChangeTide;

    private void Awake()
    {
        _onUpdateTideBar = UpdateTideBar;
        _onChangeTide = UpdateCurrentTide;
    }

    private void Start()
    {
        CrabArenaManager.Instance.OnUpdateTideTimer += _onUpdateTideBar;
        CrabArenaManager.Instance.OnStartTide += _onChangeTide;
    }

    private void OnDestroy()
    {
        CrabArenaManager.Instance.OnUpdateTideTimer -= _onUpdateTideBar;
        CrabArenaManager.Instance.OnStartTide -= _onChangeTide;
    }

    void UpdateTideBar(float current, float max)
    {
        float percent = _currentTide switch
        {
            CrabArenaState.LowTide => current / max,
            CrabArenaState.HighTide => 1 - current / max,
            CrabArenaState.IncomingTide => 1,
            CrabArenaState.OutgoingTide => 0,
            _ => 0
        };

        tidesBar.fillAmount = percent;

    }

    void UpdateCurrentTide(CrabArenaState currentTide)
    {
        _currentTide = currentTide;
        string text = _currentTide switch
        {
            CrabArenaState.LowTide => lowTideName,
            CrabArenaState.HighTide => highTideName,
            CrabArenaState.IncomingTide => lowTideName,
            CrabArenaState.OutgoingTide => highTideName,
            _ => lowTideName
        };
        tidesTitle.text = text;
    }
}
