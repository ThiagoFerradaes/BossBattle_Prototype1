using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class BattleRankManagerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankTextComponent;
    [SerializeField] Image rankImageComponent;
    [SerializeField] SerializedDictionary<BattleRank, LocalizedString> dictionaryOfStringsByRank;

    #region Start Region

    private void Awake() {
        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents() {
        BattleRankManager.OnPointGained += UpdatePoints;
        BattleRankManager.OnRankChanged += UpdateRankText;
    }

    void UnsubscribeToEvents() {
        BattleRankManager.OnPointGained -= UpdatePoints;
        BattleRankManager.OnRankChanged -= UpdateRankText;
    }

    private void Start() {
        UpdateRankText(BattleRank.E);
        UpdatePoints(0, 1);
    }

    #endregion

    void UpdatePoints(float currentPoints, float maxPoints) {
        rankImageComponent.fillAmount = currentPoints / maxPoints;
    }

    void UpdateRankText(BattleRank currentRank) {
        rankTextComponent.text = dictionaryOfStringsByRank[currentRank].GetLocalizedString();
        if (currentRank != BattleRank.SS) UpdatePoints(0, 1);
    }

}
