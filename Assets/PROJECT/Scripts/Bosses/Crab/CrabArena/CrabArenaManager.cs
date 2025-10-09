using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrabArenaManager : MonoBehaviour
{
    #region Parameters

    public static CrabArenaManager Instance;

    [Header("Components")]
    [SerializeField] CrabArenaSO arenaInfo;
    [SerializeField] Transform platformSpawnPosition;
    public CrabManager CrabM;

    // Atributes
    CrabArenaState _currentTide;
    float _currentTideTime;
    float _currentTideMaxTime;

    // Event
    public event Action<float, float> OnUpdateTideTimer;
    public event Action<CrabArenaState> OnEndTide, OnStartTide;

    // Coroutines
    Coroutine _tideTimerCoroutine;

    #endregion

    #region Initialize
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        int amountOfPlatforms = arenaInfo.ListOfPlatforms.Count;
        int randomPlatformIndex = Random.Range(0, amountOfPlatforms);
        GameObject platform = arenaInfo.ListOfPlatforms[randomPlatformIndex];

        Instantiate(platform, platformSpawnPosition.position, Quaternion.identity);
    }
    private void Start()
    {
        _currentTide = arenaInfo.InitialState;

        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());
    }
    private void OnDestroy()
    {

        // Limpando os eventos
        OnUpdateTideTimer = null;
        OnEndTide = null;
        OnStartTide = null;
    }
    #endregion

    #region Tides
    IEnumerator TideTimerCoroutine()
    {
        OnStartTide?.Invoke(_currentTide);

        DecideMaxTideDuration();

        _currentTideTime = 0;

        while (_currentTideTime < _currentTideMaxTime)
        {
            _currentTideTime += Time.deltaTime;
            OnUpdateTideTimer?.Invoke(_currentTideTime, _currentTideMaxTime);
            yield return null;
        }

        OnEndTide?.Invoke(_currentTide);

        CheckNextTide();

        _tideTimerCoroutine = null;
    }

    void DecideMaxTideDuration()
    {
        _currentTideMaxTime = _currentTide switch
        {
            CrabArenaState.LowTide => arenaInfo.DurationOfLowTide,
            CrabArenaState.IncomingTide => arenaInfo.IncomingTideDuration,
            CrabArenaState.HighTide => arenaInfo.DurationOfHeightTide,
            CrabArenaState.OutgoingTide => arenaInfo.DurationOfOutgoingTide,
            _ => arenaInfo.DurationOfLowTide
        };
    }

    void CheckNextTide()
    {
        if (_currentTide == CrabArenaState.IncomingTide || _currentTide == CrabArenaState.OutgoingTide) ChangeCurrentTide();
    }
    public void ChangeCurrentTide()
    {
        if (_tideTimerCoroutine != null) StopCoroutine(_tideTimerCoroutine);

        OnEndTide?.Invoke(_currentTide);

        Array values = Enum.GetValues(typeof(CrabArenaState));

        _currentTide = (CrabArenaState)values.GetValue(((int)_currentTide + 1) % values.Length);

        _tideTimerCoroutine = null;

        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());

    }
    #endregion

    #region Getters

    public CrabArenaState ReturnCurrentTide() => _currentTide;

    public float ReturnCurrentTidePercent() => _currentTideTime / _currentTideMaxTime;

    #endregion
}
