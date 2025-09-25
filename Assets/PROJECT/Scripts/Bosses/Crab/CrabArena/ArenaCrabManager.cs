using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaCrabManager : MonoBehaviour
{
    #region Parameters

    public static ArenaCrabManager Instance;

    [Header("Components")]
    [SerializeField] ArenaCrabSO arenaInfo;
    [SerializeField] Transform platformSpawnPosition;

    // Atributes
    CrabArenaState _currentTide;

    // Event
    public event Action OnChangeToLowTide, OnChangeToIncomingTide, OnChangeToHighTide, OnChangeToOutgoingTide;

    // Coroutines
    Coroutine _tideTimerCoroutine;

    #endregion

    #region Initialize
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        int amountOfPlatforms = arenaInfo.ListOfPlatforms.Count;
        int randomPlatformIndex = Random.Range(0, amountOfPlatforms);
        GameObject platform = arenaInfo.ListOfPlatforms[randomPlatformIndex];

        Instantiate(platform, platformSpawnPosition.position, Quaternion.identity );
    }
    private void Start() {
        _currentTide = arenaInfo.InitialState;

        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }
    private void OnDestroy() {

        // Limpando os eventos
        OnChangeToLowTide = null;
        OnChangeToIncomingTide = null;
        OnChangeToHighTide = null;
        OnChangeToOutgoingTide = null;
    }
    #endregion

    #region Tides
    IEnumerator TideTimerCoroutine() {
        float duration = _currentTide switch {
            CrabArenaState.LowTide => arenaInfo.DurationOfLowTide,
            CrabArenaState.IncomingTide => arenaInfo.IncomingTideDuration,
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

    public void ForceCurrentTideToEnd() {
        if (_tideTimerCoroutine != null) StopCoroutine(_tideTimerCoroutine);

        Array values = Enum.GetValues(typeof(CrabArenaState));

        _currentTide = (CrabArenaState)values.GetValue(((int)_currentTide + 1) % values.Length);

        Debug.Log($"Force Tide to end. The new tide is: {_currentTide}");

        _tideTimerCoroutine = null;

        switch (_currentTide) {
            case CrabArenaState.LowTide: ChangeToLowTide(); break;
            case CrabArenaState.IncomingTide: ChangeToIncomingTide(); break;
            case CrabArenaState.HighTide: ChangeToHeightTide(); break;
            case CrabArenaState.OutgoingTide: ChangeToOutgoingTide(); break;
        }

    }
    #endregion

    #region Getters

    public CrabArenaState ReturnCurrentTide() => _currentTide;

    #endregion
}
