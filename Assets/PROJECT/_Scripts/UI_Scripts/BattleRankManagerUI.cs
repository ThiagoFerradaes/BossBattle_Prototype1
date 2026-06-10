using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class BattleRankManagerUI : MonoBehaviour
{
    [Header("Rank")]
    [SerializeField] TextMeshProUGUI rankTextComponent;
    [SerializeField] Image rankImageComponent;
    [SerializeField] SerializedDictionary<BattleRank, LocalizedString> dictionaryOfStringsByRank;

    [Header("Combo")]
    [SerializeField] GameObject container;
    [SerializeField] TextMeshProUGUI comboTextComponent;
    [SerializeField] SerializedDictionary<Combo, LocalizedString> dictionaryOfStringsByCombo;

    #region Start Region

    private void Awake() {

        UpdateRankText(BattleRank.E);
        UpdatePoints(0, 1);
        TurnComboTextOff();

        SubscribeToEvents();
    }

    private void OnDestroy() {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents() {

        // Ranks
        BattleRankManager.OnPointGained += UpdatePoints;
        BattleRankManager.OnRankChanged += UpdateRankText;

        // Combos
        BattleRankManager.OnComboChanged += UpdateComboText;
        BattleRankManager.OnComboStarted += TurnComboTextOn;
        BattleRankManager.OnComboFinished += TurnComboTextOff;
    }

    void UnsubscribeToEvents() {

        // Ranks
        BattleRankManager.OnPointGained -= UpdatePoints;
        BattleRankManager.OnRankChanged -= UpdateRankText;

        // Combos
        BattleRankManager.OnComboChanged -= UpdateComboText;
        BattleRankManager.OnComboStarted -= TurnComboTextOn;
        BattleRankManager.OnComboFinished -= TurnComboTextOff;
    }

    #endregion

    #region Rank Region
    void UpdatePoints(float currentPoints, float maxPoints) {
        rankImageComponent.fillAmount = currentPoints / maxPoints;
    }

    void UpdateRankText(BattleRank currentRank) {
        rankTextComponent.text = dictionaryOfStringsByRank[currentRank].GetLocalizedString();
        if (currentRank != BattleRank.SS) UpdatePoints(0, 1);
    }

    #endregion

    #region Combo Region

    void TurnComboTextOn() {
        UpdateComboText(Combo.ComboOne);

        container.SetActive(true);
    }

    void TurnComboTextOff() {
        container.SetActive(false);

        UpdateComboText(Combo.ComboOne);
    }

    void UpdateComboText(Combo currentCombo) {
        comboTextComponent.text = dictionaryOfStringsByCombo[currentCombo].GetLocalizedString();
    }

    #endregion
}
