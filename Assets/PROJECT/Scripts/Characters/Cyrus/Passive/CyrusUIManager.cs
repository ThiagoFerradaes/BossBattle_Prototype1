using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CyrusUIManager : MonoBehaviour {

    // Components
    [Header("Rank")]
    [SerializeField] Image RankIconImage;
    [SerializeField] Image ExpProgressImage;
    [SerializedDictionary("CyrusRank", "Sprite"), SerializeField]
    SerializedDictionary<CyrusRank, Sprite> dictionaryOfRankSprites;

    [Header("Skills")]
    [SerializedDictionary("SkillSlot", "Image"), SerializeField]
    SerializedDictionary<SkillSlot, Image> dictionaryOfImages;

    // Actions
    Action _onRankUP, _onTurnLevelUpSkillOff;
    Action<float,float> _onUpdateExpProgress;

    #region Initialize
    private void Awake() {
        _onRankUP = RankUp;
        _onTurnLevelUpSkillOff = TurnLevelUpSkillOff;
        _onUpdateExpProgress = UpdateExpProgress;
    }

    private void Start() {
        CyrusPassiveManager.Instance.OnRankLevelUp += _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp  += _onTurnLevelUpSkillOff;
        CyrusPassiveManager.Instance.OnExpGain  += _onUpdateExpProgress;

        TurnLevelUpSkillOff();
    }

    private void OnDestroy() {
        CyrusPassiveManager.Instance.OnRankLevelUp -= _onRankUP;
        CyrusPassiveManager.Instance.OnSkillLevelUp -= _onTurnLevelUpSkillOff;
        CyrusPassiveManager.Instance.OnExpGain -= _onUpdateExpProgress;
    }

    #endregion

    #region UI
    void RankUp() {
        CyrusRank rank = CyrusPassiveManager.Instance.ReturnCyrusRank();
        Sprite newSprite = dictionaryOfRankSprites[rank];

        RankIconImage.sprite = newSprite;

        TurnLevelUpSkillOn();
    }

    void TurnLevelUpSkillOn() {
        foreach (var skill in dictionaryOfImages) {
            SkillSlot slot = skill.Key;
            int skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

            bool turnOn = skillLevel < 3;
            dictionaryOfImages[slot].gameObject.SetActive(turnOn);
        }
    }

    void TurnLevelUpSkillOff() {
        foreach (var skill in dictionaryOfImages) {
            skill.Value.gameObject.SetActive(false);
        }
    }

    void UpdateExpProgress(float current, float max) {
        ExpProgressImage.fillAmount = current / max;
    }
    #endregion
}
