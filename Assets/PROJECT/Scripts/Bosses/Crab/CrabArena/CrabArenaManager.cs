using System;
using System.Collections;
using System.Collections.Generic;
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
    [HideInInspector] public GameObject CrabPlatform;

    // Atributes
    CrabArenaState _currentTide;
    float _currentTideTime;
    float _currentTideMaxTime;
    Dictionary<CrabArenaState, int> _timesTideOccurred = new();

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
        GameObject randomPlatform = arenaInfo.ListOfPlatforms[randomPlatformIndex];

        GameObject plataform = Instantiate(randomPlatform, platformSpawnPosition.position, Quaternion.identity);
        CrabPlatform = plataform;
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

        if (_timesTideOccurred.ContainsKey(_currentTide)) _timesTideOccurred[_currentTide]++;
        else _timesTideOccurred[_currentTide] = 1;

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

    public void HighTideBomb()
    {
        ChangeCurrentTideTime(false, arenaInfo.BombAmountOfFlatTimeReduced);
    }

    public void ChangeCurrentTideTime(bool increase, float extraTime)
    {
        if (!increase) _currentTideTime = Mathf.Min(_currentTideTime + extraTime, _currentTideMaxTime);
        else _currentTideTime = Mathf.Max(_currentTideTime - extraTime, 0);
    }
    #endregion

    #region Getters

    public CrabArenaState ReturnCurrentTide() => _currentTide;

    public float ReturnCurrentTidePercent() => _currentTideTime / _currentTideMaxTime;

    public float ReturnCurrentTideRemainingTime() => _currentTideMaxTime - _currentTideTime;

    public int ReturnAmountOfTideOccurence(CrabArenaState tide)
    {
        if (_timesTideOccurred.ContainsKey(tide)) return _timesTideOccurred[tide];
        else return 0;
    }

    #endregion
}
