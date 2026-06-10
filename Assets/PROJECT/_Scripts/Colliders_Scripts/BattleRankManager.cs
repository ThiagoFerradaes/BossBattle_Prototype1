using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class BattleRankManager : MonoBehaviour {

    // Manager atributes
    float _totalScorePoints;
    float _lastTimePlayerGotHit;
    BattleRank _currentRank = BattleRank.E;

    // Future scriptable
    public SerializedDictionary<BattleRank, float> dictionaryOfRanksPoints;
    public float minTimeMultiplierValue, maxTimeMultiplierValue, maxTimeNoDamageTaken;
    public LayerMask enemyLayer, playerLayer;


    // Eventos
    /// <summary>
    /// First float is current points, second float is max point in that tank
    /// </summary>
    public static event Action<float, float> OnPointGained;
    public static event Action<BattleRank> OnRankChanged;

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
            _totalScorePoints += 10 * ReturnTimeNoDamageTakenMultiplier();
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

    private void SetLastTimePlayerGotHit() {
        _lastTimePlayerGotHit = Time.time;
    }

    float ReturnTimeNoDamageTakenMultiplier() {
        float currentTimeWithoutTakingDamage = Time.time - _lastTimePlayerGotHit;
        float currentTimeAvarege = Mathf.Clamp01(currentTimeWithoutTakingDamage / maxTimeNoDamageTaken);
        float currentMultiplier = Mathf.Lerp(minTimeMultiplierValue, maxTimeMultiplierValue, currentTimeAvarege);

        return currentMultiplier;
    }
}
