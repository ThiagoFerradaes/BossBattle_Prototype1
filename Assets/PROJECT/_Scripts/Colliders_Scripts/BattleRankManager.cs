using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using UnityEngine;

public class BattleRankManager : MonoBehaviour {

    // Manager atributes
    float _totalScorePoints;
    float _lastTimePlayerGotHit;
    float _lastTimeAddedToCombo;
    int _comboIndex;
    bool _canStartCombo = true;
    Combo _currentCombo = Combo.ComboOne;
    BattleRank _currentRank = BattleRank.E;

    Coroutine _comboDurationCoroutine;

    // Future scriptable
    [Tooltip("The total amount of hits to get to the next rank")] public SerializedDictionary<BattleRank, float> dictionaryOfRanksPoints;
    [Tooltip("The total amount of hits to get to the next combo")] public SerializedDictionary<Combo, int> dictionaryOfHitsPerCombo;
    [Tooltip("The multiplier based on the current combo")] public SerializedDictionary<Combo, float> dictionaryOfMultipliersByCombo;
    public float minTimeMultiplierValue, maxTimeMultiplierValue, maxTimeNoDamageTaken;
    public float comboCooldown, comboMaxDuration;
    public LayerMask enemyLayer, playerLayer;


    // Eventos
    /// <summary>
    /// First float is current points, second float is max point in that tank
    /// </summary>
    public static event Action<float, float> OnPointGained;
    public static event Action<BattleRank> OnRankChanged;
    public static event Action<Combo> OnComboChanged;
    public static event Action OnComboStarted, OnComboFinished;

    #region Start Region
    private void Awake() {
        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents() {
        HitBox.OnHitTarget += GainPoints;
    }

    void UnsubscribeToEvents() {
        HitBox.OnHitTarget -= GainPoints;
    }

    private void Start() {
        SetLastTimePlayerGotHit();
    }

    #endregion

    private void GainPoints(LayerMask obj) {

        if (enemyLayer.ContainsLayer(obj)) {

            AddToCombo();

            _comboDurationCoroutine ??= StartCoroutine(ComboDurationTimer());

            float timePoints = 10 * ReturnTimeNoDamageTakenMultiplier();
            float comboM =  10 * ReturnComboMultiplier();
            float scoreAdd = timePoints + comboM;

            _totalScorePoints += scoreAdd;

            Debug.Log($"Total scored recieved: {scoreAdd} TimePoints: {timePoints} ComboPoints: {comboM}");

            CheckPoints();
        }

        else if (playerLayer.ContainsLayer(obj)) {
            SetLastTimePlayerGotHit();
        }

    }

    void CheckPoints() {
        if (_currentRank >= BattleRank.SS) {
            return;
        }

        float currentRankLowValue = _currentRank <= BattleRank.E ? 0 : dictionaryOfRanksPoints[_currentRank - 1];
        float currentRankMaxValue = dictionaryOfRanksPoints[_currentRank] - currentRankLowValue;
        float currentRankPoint = _totalScorePoints - currentRankLowValue;

        OnPointGained?.Invoke(currentRankPoint, currentRankMaxValue);

        if (_totalScorePoints >= dictionaryOfRanksPoints[_currentRank]) {
            _currentRank++;
            OnRankChanged?.Invoke(_currentRank);
        }
    }

    #region Time without taking damage region
    private void SetLastTimePlayerGotHit() {
        _lastTimePlayerGotHit = Time.time;
    }

    float ReturnTimeNoDamageTakenMultiplier() {
        float currentTimeWithoutTakingDamage = Time.time - _lastTimePlayerGotHit;
        float currentTimeAvarege = Mathf.Clamp01(currentTimeWithoutTakingDamage / maxTimeNoDamageTaken);
        float currentMultiplier = Mathf.Lerp(minTimeMultiplierValue, maxTimeMultiplierValue, currentTimeAvarege);

        return currentMultiplier;
    }

    #endregion

    #region Combo multiplier

    float ReturnComboMultiplier() {
        return dictionaryOfMultipliersByCombo[_currentCombo];
    }

    void AddToCombo() {
        if (!_canStartCombo) return;

        _lastTimeAddedToCombo = Time.time;

        _comboIndex++;

        if (_currentCombo >= Combo.ComboFive) return;

        if (_comboIndex >= dictionaryOfHitsPerCombo[_currentCombo]) {
            _currentCombo++;
            OnComboChanged?.Invoke(_currentCombo);
        }
    }

    IEnumerator ComboDurationTimer() {

        OnComboStarted?.Invoke();

        while (Time.time - _lastTimeAddedToCombo < comboMaxDuration) yield return null;

        OnComboFinished?.Invoke();

        _canStartCombo = false;
        _currentCombo = Combo.ComboOne;
        _comboIndex = 0;

        float endTime = Time.time;

        while (Time.time - endTime < comboCooldown) yield return null;

        _canStartCombo = true;
        _comboDurationCoroutine = null;
    }


    #endregion
}
