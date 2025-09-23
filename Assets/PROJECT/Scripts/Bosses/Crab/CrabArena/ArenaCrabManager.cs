using System;
using System.Collections;
using UnityEngine;

public class ArenaCrabManager : MonoBehaviour
{
    [SerializeField] ArenaCrabSO arenaInfo;

    CrabArenaState _currentTide;

    // Event
    public event Action OnChangeToLowTide, OnChangeToIncomingTide, OnChangeToHighTide, OnChangeToOutgoingTide;

    // Coroutines
    Coroutine _tideTimerCoroutine;

    private void Start() {
        _currentTide = arenaInfo.InitialState;

        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }

    IEnumerator TideTimerCoroutine() {
        float duration = _currentTide switch {
            CrabArenaState.LowTide => arenaInfo.DurationOfLowTide,
            CrabArenaState.IncomingTide => arenaInfo.DurationOfIncomingTide,
            CrabArenaState.HighTide => arenaInfo.DurationOfHeightTide,
            CrabArenaState.OutgoingTide => arenaInfo.DurationOfOutgoingTide,
            _ => arenaInfo.DurationOfLowTide
        };

        Debug.Log($"The current tide is: {_currentTide}, it will last: {duration}");
        yield return new WaitForSeconds(duration);

        Array values = Enum.GetValues(typeof(CrabArenaState));

        _currentTide = (CrabArenaState)values.GetValue(((int)_currentTide + 1) % values.Length);

        Debug.Log($"The new tide is: {_currentTide}");

        _tideTimerCoroutine = null;

        switch (_currentTide) {
            case CrabArenaState.LowTide: ChangeToLowTide(); break;
            case CrabArenaState.IncomingTide: ChangeToIncomingTide(); break;
            case CrabArenaState.HighTide: ChangeToHeightTide(); break;
            case CrabArenaState.OutgoingTide: ChangeToOutgoingTide(); break;
        }

    }

    void ChangeToLowTide() {
        OnChangeToLowTide?.Invoke();
        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }

    void ChangeToIncomingTide() {
        OnChangeToIncomingTide?.Invoke();
        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }

    void ChangeToHeightTide() {
        OnChangeToHighTide?.Invoke();
        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }
    void ChangeToOutgoingTide() {
        OnChangeToOutgoingTide?.Invoke();
        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }
}
