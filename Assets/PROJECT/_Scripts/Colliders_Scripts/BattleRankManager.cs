using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class BattleRankManager : MonoBehaviour {
    float _totalScorePoints;
    BattleRank _currentRank = BattleRank.E;
    public SerializedDictionary<BattleRank, float> dictionaryOfRanksPoints;
    public LayerMask enemyLayer, playerLayer;


    // Eventos
    /// <summary>
    /// First float is current points, second float is max point in that tank
    /// </summary>
    public static event Action<float, float> OnPointGained;
    public static event Action<BattleRank> OnRankChanged;

    private void Awake() {
        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents() {
        HitBox.OnHitTarget += HitBox_OnHitTarget;
    }

    void UnsubscribeToEvents() {
        HitBox.OnHitTarget -= HitBox_OnHitTarget;
    }

    private void HitBox_OnHitTarget(LayerMask obj) {

        if (enemyLayer.ContainsLayer(obj)) {
            _totalScorePoints += 10;
            CheckPoints();
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
}
