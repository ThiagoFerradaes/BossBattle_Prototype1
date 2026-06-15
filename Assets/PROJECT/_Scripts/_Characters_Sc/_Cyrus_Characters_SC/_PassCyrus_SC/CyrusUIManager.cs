using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CyrusUIManager : MonoBehaviour {

    // Components
    [Header("Rank")]
    [SerializeField] Image rankProgresBar;
    [SerializeField] GameObject rankLevelUpImage;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] GameObject maxRankAnimation;
    [SerializeField] Animator rankLevelUpAnimator;
    [SerializeField] float levelUpTime, animationDuration;
    [SerializeField] string animationStateName;

    [Header("Skills")]
    [SerializedDictionary("SkillSlot", "Object"), SerializeField]
    SerializedDictionary<SkillSlot, GameObject> dictionaryOfImages;

    // Actions
    Action<BattleRank> _onRankUP;

    WaitForSeconds levelUpWaitForSeconds, animationWaitForSeconds;

    Coroutine animationCooldownRoutine;

    #region Initialize
    private void Awake() {
        _onRankUP = RankUp;

        levelUpWaitForSeconds = new(levelUpTime);
        animationWaitForSeconds = new(animationDuration);
    }

    private void Start() {
        BattleRankManager.OnRankChanged += _onRankUP;

        TurnLevelUpSkillOff();

        maxRankAnimation.SetActive(false);
        rankLevelUpImage.SetActive(false);
    }

    private void OnDestroy() {
        BattleRankManager.OnRankChanged -= _onRankUP;
    }

    #endregion

    #region UI
    void RankUp(BattleRank currentRank) {
        float amountOfRanks = Enum.GetValues(typeof(BattleRank)).Length - 1;
        rankProgresBar.fillAmount = (float)currentRank / amountOfRanks;

        if ((int)currentRank == amountOfRanks) {
            maxRankAnimation.SetActive(true);
        }
    }

    void TurnLevelUpSkillOff() {
        RankUp(BattleRank.E);
        foreach (var skill in dictionaryOfImages) {
            skill.Value.SetActive(false);
        }
    }
    #endregion
}
