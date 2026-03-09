using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
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
    [SerializedDictionary("Type", "SkillBackground"), SerializeField] SerializedDictionary<SkillType, Image> dictionaryOfSkillBackgrounds = new();
    [SerializedDictionary("Type", "Button"), SerializeField] SerializedDictionary<SkillType, Button> dictionaryOfSkillButtons = new();
    [SerializeField] Color selectedSkillColor;
    [SerializeField] Color unselectedSkillColor;

    SkillSlot _currentSlot;
    CharacterSelectionManager _characterSelectionManager;

    public void Awake() {
        _characterSelectionManager = GetComponent<CharacterSelectionManager>();
        SetButtons();
    }
    void SetButtons() {
        closeSelectionScreen.onClick.AddListener(TurnScreenOff);

        foreach (var button in dictionaryOfSkillButtons) {
            var slot = button.Key;
            button.Value.onClick.AddListener(() => ChangeSelectedSkill(slot));
        }
    }
    public void Initialize(SkillSlot slotInitialized) {

        ChangeCurrentSlot(slotInitialized);

        ChangeIconsAndInformations();

        SetConexions(slotInitialized);
        skillSelectionScreen.SetActive(true);
    }

    public void ChangeCurrentSlot(SkillSlot slot) {
        _currentSlot = slot;
    }
    public void ChangeIconsAndInformations() {
        switch (_currentSlot) {
            case SkillSlot.Passive:
                SetPassive(); break;
            default:
                SetSkill(); break;
        }
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
    void SetSkill() {

        passiveIconObject.SetActive(false);

        List<SkillUnlockedInfo> skilslInfo = WhiteBoard.Instance.ReturnCurrentCharacterSkillsBySlot(_currentSlot);

        SkillUnlockedInfo classicSkill = skilslInfo.Where(p => p.Type == SkillType.Classic).FirstOrDefault();
        SkillUnlockedInfo alternativeSkill = skilslInfo.Where(p => p.Type == SkillType.Alternative).FirstOrDefault();

        // Classic skill
        if (classicSkill != null) {

            // Locks
            dictionaryOfLocks[SkillType.Classic].gameObject.SetActive(!classicSkill.IsUnlocked);

            // Button
            dictionaryOfSkillButtons[SkillType.Classic].interactable = classicSkill.IsUnlocked;

            // Icons
            dictionaryOfSkillIcons[SkillType.Classic].sprite = classicSkill.Skill.UISkillSpriteIcon;

        }

        // Alternative skill
        if (alternativeSkill != null) {

            // Locks
            dictionaryOfLocks[SkillType.Alternative].gameObject.SetActive(!alternativeSkill.IsUnlocked);

            // Button
            dictionaryOfSkillButtons[SkillType.Alternative].interactable = alternativeSkill.IsUnlocked;

            // Icons
            dictionaryOfSkillIcons[SkillType.Alternative].sprite = alternativeSkill.IsUnlocked ?
                alternativeSkill.Skill.UISkillSpriteIcon : alternativeSkill.Skill.MapLockSkillSpriteIcon;

        }
        else {
            // Locks
            dictionaryOfLocks[SkillType.Alternative].gameObject.SetActive(true);

            // Button
            dictionaryOfSkillButtons[SkillType.Alternative].interactable = false;
        }

        // Descrição
        Character currentCharacter = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();
        SkillSO skill = _currentSlot switch {
            SkillSlot.SkillOne => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(currentCharacter),
            SkillSlot.SkillTwo => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillTwo(currentCharacter),
            SkillSlot.Ultimate => CurrentSelectedCharacterWhiteBoard.Instance.ReturnUltimate(currentCharacter),
            _ => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(currentCharacter)
        };

        ChangeDescriptionText(skill.SkillLongDescription, skill.SkillName);
        ChangeSkillBackground(skill.SkillType);

        // Object
        skillsIconObject.SetActive(true);
    }

    void ChangeSelectedSkill(SkillType typeOfSkill) {

        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(_currentSlot);
        List<SkillUnlockedInfo> skilslInfo = WhiteBoard.Instance.ReturnCurrentCharacterSkillsBySlot(_currentSlot);

        SkillUnlockedInfo newSkill = skilslInfo.Where(p => p.Type == typeOfSkill).FirstOrDefault();

        if (currentSkill == newSkill.Skill || newSkill == null) return;

        CurrentSelectedCharacterWhiteBoard.Instance.SetCurrentCharacterSkillBySlot(_currentSlot, newSkill.Skill);

        ChangeDescriptionText(newSkill.Skill.SkillLongDescription, newSkill.Skill.SkillName);

        ChangeSkillBackground(typeOfSkill);

        _characterSelectionManager.ChangeSkillIcon(_currentSlot);
    }
    #endregion

    void SetConexions(SkillSlot slot) {
        foreach (var conexion in dictionaryOfConexions)
            conexion.Value.gameObject.SetActive(conexion.Key == slot);
    }

    void ChangeSkillBackground(SkillType type) {
        foreach (var background in dictionaryOfSkillBackgrounds) {
            if (background.Key == type) background.Value.color = selectedSkillColor;
            else background.Value.color = unselectedSkillColor;
        }
    }
    void ChangeDescriptionText(string text, string name) {
        skillLongDescription.text = text;
        skillName.text = name;
    }

    public void TurnScreenOff() {
        passiveIconObject.SetActive(false);
        skillsIconObject.SetActive(false);
        skillSelectionScreen.SetActive(false);

        _characterSelectionManager.TurnOffSkillSelectionBackground();
    }
}
