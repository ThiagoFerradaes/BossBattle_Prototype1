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
    [SerializeField] List<LocalizedString> rankTexts;
    [SerializeField] GameObject maxRankAnimation;
    [SerializeField] Animator rankLevelUpAnimator;
    [SerializeField] float levelUpTime, animationDuration;
    [SerializeField] string animationStateName;

    [Header("Skills")]
    [SerializedDictionary("SkillSlot", "Object"), SerializeField]
    SerializedDictionary<SkillSlot, GameObject> dictionaryOfImages;

    // Actions
    Action<SkillSlot> _onSkillLevelUp;
    Action<float, float> _onRankUP;

    WaitForSeconds levelUpWaitForSeconds, animationWaitForSeconds;

    Coroutine animationCooldownRoutine;

    #region Initialize
    private void Awake() {
        _onRankUP = RankUp;
        _onSkillLevelUp = SkillLevelUp;

        levelUpWaitForSeconds = new(levelUpTime);
        animationWaitForSeconds = new(animationDuration);
    }

    private void Start() {
        CyrusPassiveManager.Instance.OnRankLevelUp += _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp  += _onSkillLevelUp;

        TurnLevelUpSkillOff();

        maxRankAnimation.SetActive(false);
        rankLevelUpImage.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        CyrusPassiveManager.Instance.OnRankLevelUp -= _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp -= _onSkillLevelUp;
    }

    #endregion

    #region UI
    void RankUp(float currentRank, float maxRank) {
        rankProgresBar.fillAmount = currentRank/maxRank;
        UpdateText(currentRank);

        if (currentRank == maxRank) {
            maxRankAnimation.SetActive(true);
        }

        StartCoroutine(SkillLevelUpTimer(rankLevelUpImage));
    }
    private void UpdateText(float currentRank) {
        rankText.text = rankTexts[(int)currentRank].GetLocalizedString();
    }
    void TurnLevelUpSkillOff() {
        RankUp(0, 1);
        foreach (var skill in dictionaryOfImages) {
            skill.Value.SetActive(false);
        }
    }

    void SkillLevelUp(SkillSlot slot)
    {
        StartCoroutine(SkillLevelUpTimer(dictionaryOfImages[slot]));

        animationCooldownRoutine ??= StartCoroutine(AnimationCooldown());
    }

    IEnumerator SkillLevelUpTimer(GameObject image)
    {
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
