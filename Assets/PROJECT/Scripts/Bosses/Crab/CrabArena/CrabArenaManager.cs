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

    // Atributes
    CrabArenaState _currentTide;
    float _currentTideTime;
    float _currentTideMaxTime;

    // Event
    public event Action<float, float> OnUpdateTideTimer;
    public event Action<CrabArenaState> OnChangeTide;

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
        OnChangeTide = null;
    }
    #endregion

    #region Tides
    IEnumerator TideTimerCoroutine()
    {
        while (true)
        {
            _currentTideMaxTime = _currentTide switch
            {
                CrabArenaState.LowTide => arenaInfo.DurationOfLowTide,
                CrabArenaState.IncomingTide => arenaInfo.IncomingTideDuration,
                CrabArenaState.HighTide => arenaInfo.DurationOfHeightTide,
                CrabArenaState.OutgoingTide => arenaInfo.DurationOfOutgoingTide,
                _ => arenaInfo.DurationOfLowTide
            };

            _currentTideTime = 0;

            while (_currentTideTime < _currentTideMaxTime)
            {
                _currentTideTime += Time.deltaTime;
                OnUpdateTideTimer?.Invoke(_currentTideTime, _currentTideMaxTime);
                yield return null;
            }

            Array values = Enum.GetValues(typeof(CrabArenaState));

            _currentTide = (CrabArenaState)values.GetValue(((int)_currentTide + 1) % values.Length);

            OnChangeTide?.Invoke(_currentTide);
        }

    }
    public void ForceCurrentTideToEnd()
    {
        if (_tideTimerCoroutine != null) StopCoroutine(_tideTimerCoroutine);

        Array values = Enum.GetValues(typeof(CrabArenaState));

        _currentTide = (CrabArenaState)values.GetValue(((int)_currentTide + 1) % values.Length);

        _tideTimerCoroutine = null;

        OnChangeTide?.Invoke(_currentTide);

        _tideTimerCoroutine ??= StartCoroutine(TideTimerCoroutine());

    }
    #endregion

    #region Getters

    public CrabArenaState ReturnCurrentTide() => _currentTide;

    public float ReturnCurrentTidePercent() => _currentTideTime / _currentTideMaxTime;

    #endregion
}
