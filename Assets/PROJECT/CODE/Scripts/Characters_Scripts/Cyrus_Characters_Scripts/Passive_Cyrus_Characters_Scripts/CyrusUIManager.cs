using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CyrusUIManager : MonoBehaviour {

    // Components
    [Header("Rank")]
    [SerializeField] Image rankProgresBar;

    [Header("Skills")]
    [SerializedDictionary("SkillSlot", "Image"), SerializeField]
    SerializedDictionary<SkillSlot, Image> dictionaryOfImages;

    // Actions
    Action<SkillSlot> _onSkillLevelUp;
    Action<float, float> _onRankUP;

    #region Initialize
    private void Awake() {
        _onRankUP = RankUp;
        _onSkillLevelUp = SkillLevelUp;
    }

    private void Start() {
        CyrusPassiveManager.Instance.OnRankLevelUp += _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp  += _onSkillLevelUp;

        TurnLevelUpSkillOff();
    }

    private void OnDestroy() {
        CyrusPassiveManager.Instance.OnRankLevelUp -= _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp -= _onSkillLevelUp;
    }

    #endregion

    #region UI
    void RankUp(float currentRank, float maxRank) {
        rankProgresBar.fillAmount = currentRank/maxRank;
    }

    void TurnLevelUpSkillOff() {
        RankUp(0, 1);
        foreach (var skill in dictionaryOfImages) {
            skill.Value.gameObject.SetActive(false);
        }
    }

    void SkillLevelUp(SkillSlot slot)
    {
        StartCoroutine(SkillLevelUpTimer(dictionaryOfImages[slot]));
    }

    IEnumerator SkillLevelUpTimer(Image image)
    {
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        image.gameObject.SetActive(false);
    }
    #endregion
}
