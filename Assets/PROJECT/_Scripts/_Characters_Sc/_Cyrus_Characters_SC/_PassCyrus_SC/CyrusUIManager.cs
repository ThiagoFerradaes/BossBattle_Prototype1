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
    //[SerializeField] SerializedDictionary<BattleRank, LocalizedString> rankTexts;
    [SerializeField] GameObject maxRankAnimation;
    [SerializeField] Animator rankLevelUpAnimator;
    [SerializeField] float levelUpTime, animationDuration;
    [SerializeField] string animationStateName;

    [Header("Skills")]
    [SerializedDictionary("SkillSlot", "Object"), SerializeField]
    SerializedDictionary<SkillSlot, GameObject> dictionaryOfImages;

    // Actions
    Action<SkillSlot> _onSkillLevelUp;
    Action<BattleRank> _onRankUP;

    WaitForSeconds levelUpWaitForSeconds, animationWaitForSeconds;

    Coroutine animationCooldownRoutine;

    #region Initialize
    private void Awake() {
        _onRankUP = RankUp;
        //_onSkillLevelUp = SkillLevelUp;

        levelUpWaitForSeconds = new(levelUpTime);
        animationWaitForSeconds = new(animationDuration);
    }

    private void Start() {
        BattleRankManager.OnRankChanged += _onRankUP;
        //CyrusPassiveManager.Instance.OnRankLevelUp += _onRankUP;
        //CyrusPassiveManager.Instance.OnSkillLevelUp  += _onSkillLevelUp;

        TurnLevelUpSkillOff();

        maxRankAnimation.SetActive(false);
        rankLevelUpImage.SetActive(false);
    }

    private void OnDestroy() {
        BattleRankManager.OnRankChanged -= _onRankUP;
        //CyrusPassiveManager.Instance.OnRankLevelUp -= _onRankUP;
        //CyrusPassiveManager.Instance.OnSkillLevelUp -= _onSkillLevelUp;
    }

    #endregion

    #region UI
    void RankUp(BattleRank currentRank) {
        float amountOfRanks = Enum.GetValues(typeof(BattleRank)).Length;
        rankProgresBar.fillAmount = (float)currentRank / amountOfRanks;
        //UpdateText(currentRank);

        if ((int)currentRank == amountOfRanks) {
            maxRankAnimation.SetActive(true);
        }

        //StartCoroutine(SkillLevelUpTimer(rankLevelUpImage));
    }
    //private void UpdateText(BattleRank currentRank) {
    //    rankText.text = rankTexts[currentRank].GetLocalizedString();
    //}
    void TurnLevelUpSkillOff() {
        RankUp(BattleRank.E);
        foreach (var skill in dictionaryOfImages) {
            skill.Value.SetActive(false);
        }
    }

    void SkillLevelUp(SkillSlot slot) {
        StartCoroutine(SkillLevelUpTimer(dictionaryOfImages[slot]));

        animationCooldownRoutine ??= StartCoroutine(AnimationCooldown());
    }

    IEnumerator SkillLevelUpTimer(GameObject image) {
        image.SetActive(true);
        yield return levelUpWaitForSeconds;
        image.SetActive(false);
    }

    IEnumerator AnimationCooldown() {
        rankLevelUpAnimator.CrossFade(animationStateName, 0);
        yield return animationWaitForSeconds;
        animationCooldownRoutine = null;
    }
    #endregion
}
