using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionManager : MonoBehaviour {

    [Header("Components")]
    [SerializeField] GameObject skillSelectionScreen;
    [SerializeField] Button closeSelectionScreen;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillLongDescription;
    [SerializedDictionary("Slot", "Conexion"), SerializeField]
    SerializedDictionary<SkillSlot, Image> dictionaryOfConexions = new();

    [Header("Passive")]
    [SerializeField] GameObject passiveIconObject;
    [SerializeField] Image passiveIcon;

    [Header("Skill")]
    [SerializeField] GameObject skillsIconObject;
    [SerializedDictionary("Type", "LockImage"), SerializeField] SerializedDictionary<SkillType, Image> dictionaryOfLocks = new(); 
    [SerializedDictionary("Type", "SkillIcon"), SerializeField] SerializedDictionary<SkillType, Image> dictionaryOfSkillIcons = new();

    public void Awake() {
        SetButtons();
    }
    public void Initialize(SkillSlot slotInitialized) {

        switch (slotInitialized) {
            case SkillSlot.Passive:
                SetPassive(); break;
            default:
                SetSkill(slotInitialized); break;
        }

        SetConexions(slotInitialized);
        skillSelectionScreen.SetActive(true);
    }

    void SetButtons() {
        closeSelectionScreen.onClick.AddListener(TurnScreenOff);
    }
    #region Passive
    void SetPassive() {
        skillsIconObject.SetActive(false);

        Character currentCharater = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();
        PassiveSO passive = CurrentSelectedCharacterWhiteBoard.Instance.ReturnPassive(currentCharater);

        passiveIcon.sprite = passive.PassiveIcon;
        ChangeDescriptionText(passive.LongDescription, passive.PassiveName);

        passiveIconObject.SetActive(true);
    }
    #endregion

    #region Skills
    void SetSkill(SkillSlot slot) {

        passiveIconObject.SetActive(false);

        List<SkillUnlockedInfo> skilslInfo = WhiteBoard.Instance.ReturnCurrentCharacterSkillsBySlot(slot);

        SkillUnlockedInfo classicSkill = skilslInfo.Where(p => p.Type == SkillType.Classic).First();
        SkillUnlockedInfo alternativeSkill = skilslInfo.Where(p => p.Type == SkillType.Alternative).First();

        // Locks
        dictionaryOfLocks[SkillType.Classic].gameObject.SetActive(!classicSkill.IsUnlocked); 
        dictionaryOfLocks[SkillType.Alternative].gameObject.SetActive(!alternativeSkill.IsUnlocked);

        // Icons
        dictionaryOfSkillIcons[SkillType.Classic].sprite = classicSkill.Skill.SkillSpriteIcon;
        dictionaryOfSkillIcons[SkillType.Alternative].sprite = alternativeSkill.Skill.SkillSpriteIcon;

        // Descrição
        Character currentCharacter = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();
        SkillSO skill = slot switch {
            SkillSlot.SkillOne => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(currentCharacter),
            SkillSlot.SkillTwo => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillTwo(currentCharacter),
            SkillSlot.Ultimate => CurrentSelectedCharacterWhiteBoard.Instance.ReturnUltimate(currentCharacter),
            _ => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(currentCharacter)
        };

        ChangeDescriptionText(skill.SkillLongDescription, skill.SkillName);

        // Object
        skillsIconObject.SetActive(true);
    }
    #endregion

    void SetConexions(SkillSlot slot) {
        foreach (var conexion in dictionaryOfConexions)
            conexion.Value.gameObject.SetActive(conexion.Key == slot);
    }
    
    void ChangeDescriptionText(string text, string name) {
        skillLongDescription.text = text;
        skillName.text = name;
    }

    void TurnScreenOff() {
        passiveIconObject.SetActive(false);
        skillsIconObject.SetActive(false);
        skillSelectionScreen.SetActive(false);
    }
}
