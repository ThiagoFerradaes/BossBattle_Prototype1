using AYellowpaper.SerializedCollections;
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
    [SerializeField] Image alternativeLockImage;
    [SerializedDictionary("Type", "SkillIcon"), SerializeField] SerializedDictionary<SkillType, Image> dictionaryOfSkillIcons = new();
    [SerializedDictionary("Type", "SkillBackground"), SerializeField] SerializedDictionary<SkillType, Image> dictionaryOfSkillBackgrounds = new();
    [SerializedDictionary("Type", "Button"), SerializeField] SerializedDictionary<SkillType, Button> dictionaryOfSkillButtons = new();


    SkillUnlockedInfo _classicSkillUnlockedInfo, _alternativeSkillUnlockedInfo;
    SkillSlot _currentSlot;
    CharacterSelectionManager _characterSelectionManager;

    #region Awake Region
    public void Awake() {

        _characterSelectionManager = GetComponent<CharacterSelectionManager>();
        SetButtons();

    }
    void SetButtons() {

        closeSelectionScreen.onClick.AddListener(TurnScreenOff);

        foreach (var button in dictionaryOfSkillButtons) {
            var typeOfSkill = button.Key;
            button.Value.onClick.AddListener(() => ChangeSelectedSkill(typeOfSkill));
        }

    }
    public void TurnScreenOff() {
        passiveIconObject.SetActive(false);
        skillsIconObject.SetActive(false);
        skillSelectionScreen.SetActive(false);

        _characterSelectionManager.TurnCloseButtonOn();
        _characterSelectionManager.TurnOffSkillSelectionBackground();
    }

    /// <summary>
    /// Troca a skill selecionada
    /// </summary>
    /// <param name="typeOfSkill"></param>
    void ChangeSelectedSkill(SkillType typeOfSkill) {

        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(_currentSlot);
        List<SkillUnlockedInfo> skilslInfo = WhiteBoard.Instance.ReturnCurrentCharacterSkillsBySlot(_currentSlot);

        SkillUnlockedInfo newSkill = skilslInfo.Where(p => p.Type == typeOfSkill).FirstOrDefault();

        if (currentSkill == newSkill.Skill || newSkill == null) return;

        CurrentSelectedCharacterWhiteBoard.Instance.SetCurrentCharacterSkillBySlot(_currentSlot, newSkill.Skill);

        ChangeDescriptionText(newSkill.Skill.MapDescriptionInfo.SkillLongDescription.GetLocalizedString(), 
            newSkill.Skill.MapDescriptionInfo.SkillName.GetLocalizedString());

        ChangeSkillBackground(typeOfSkill);

        ChangeSelectedSkillIcon(typeOfSkill);

        _characterSelectionManager.ChangeSkillIcon(_currentSlot);
    }
    #endregion

    #region Initialize Region
    public void Initialize(SkillSlot slotInitialized) {

        ChangeCurrentSlot(slotInitialized);

        SetConexions(slotInitialized);

        ChangeIconsAndInformations();

        skillSelectionScreen.SetActive(true);
    }

    public void ChangeCurrentSlot(SkillSlot slot) {
        _currentSlot = slot;
    }

    void SetConexions(SkillSlot slot) {
        foreach (var conexion in dictionaryOfConexions)
            conexion.Value.gameObject.SetActive(conexion.Key == slot);
    }

    public void ChangeIconsAndInformations() {
        switch (_currentSlot) {
            case SkillSlot.Passive:
                SetPassive(); break;
            default:
                SetSkillComponentInfo(); break;
        }
    }

    #region Passive
    void SetPassive() {
        skillsIconObject.SetActive(false);

        Character currentCharater = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();
        PassiveSO passive = CurrentSelectedCharacterWhiteBoard.Instance.ReturnPassive(currentCharater);

        passiveIcon.sprite = passive.PassiveIcon;
        ChangeDescriptionText(passive.LongDescription.GetLocalizedString(), passive.PassiveName.GetLocalizedString());

        passiveIconObject.SetActive(true);
    }
    #endregion

    #region Skills
    /// <summary>
    /// Settando as informações das skills
    /// </summary>
    void SetSkillComponentInfo() {

        passiveIconObject.SetActive(false);

        List<SkillUnlockedInfo> skilslInfo = WhiteBoard.Instance.ReturnCurrentCharacterSkillsBySlot(_currentSlot);

        _classicSkillUnlockedInfo = skilslInfo.Where(p => p.Type == SkillType.Classic).FirstOrDefault();
        _alternativeSkillUnlockedInfo = skilslInfo.Where(p => p.Type == SkillType.Alternative).FirstOrDefault();

        SkillSO skillInfo = _currentSlot switch {
            SkillSlot.SkillOne => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(),
            SkillSlot.SkillTwo => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillTwo(),
            SkillSlot.Ultimate => CurrentSelectedCharacterWhiteBoard.Instance.ReturnUltimate(),
            _ => CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne()
        };

        #region Skill icons
        // Classic skill
        if (_classicSkillUnlockedInfo != null) {

            // Button
            dictionaryOfSkillButtons[SkillType.Classic].interactable = _classicSkillUnlockedInfo.IsUnlocked;

            // Icons
            dictionaryOfSkillIcons[SkillType.Classic].sprite = _classicSkillUnlockedInfo.Skill.MapDescriptionInfo.UISkillSpriteIcon;
        }

        // Alternative skill
        if (_alternativeSkillUnlockedInfo != null) {

            // Locks
            alternativeLockImage.gameObject.SetActive(!_alternativeSkillUnlockedInfo.IsUnlocked);

            // Button
            dictionaryOfSkillButtons[SkillType.Alternative].interactable = _alternativeSkillUnlockedInfo.IsUnlocked;

            // Icons
            dictionaryOfSkillIcons[SkillType.Alternative].sprite = _alternativeSkillUnlockedInfo.IsUnlocked ?
                _alternativeSkillUnlockedInfo.Skill.MapDescriptionInfo.UISkillSpriteIcon : _alternativeSkillUnlockedInfo.Skill.MapDescriptionInfo.MapLockSkillSpriteIcon;
        }

        // Icone da skill selecionada
        ChangeSelectedSkillIcon(skillInfo.SkillType);
        #endregion

        // Descrição
        ChangeDescriptionText(skillInfo.MapDescriptionInfo.SkillLongDescription.GetLocalizedString(), 
            skillInfo.MapDescriptionInfo.SkillName.GetLocalizedString());

        // Moldura
        ChangeSkillBackground(skillInfo.SkillType);

        // Object
        skillsIconObject.SetActive(true);
    }

    void ChangeSelectedSkillIcon(SkillType type) {
        SkillSO skill;
        foreach (var skillImage in dictionaryOfSkillIcons) {
            switch (skillImage.Key) {
                case SkillType.Classic:
                    skill = _classicSkillUnlockedInfo.Skill;
                    skillImage.Value.sprite = type == SkillType.Classic ? skill.MapDescriptionInfo.MapSkillSelectedSpriteIcon : skill.MapDescriptionInfo.MapSkillSpriteIcon;
                    break;
                case SkillType.Alternative:
                    skill = _alternativeSkillUnlockedInfo.Skill;
                    if (!_alternativeSkillUnlockedInfo.IsUnlocked) skillImage.Value.sprite = skill.MapDescriptionInfo.MapLockSkillSpriteIcon;
                    else if (type == SkillType.Alternative) skillImage.Value.sprite = skill.MapDescriptionInfo.MapSkillSelectedSpriteIcon;
                    else skillImage.Value.sprite = skill.MapDescriptionInfo.MapSkillSpriteIcon;
                    break;
            }
        }
    }

    void ChangeSkillBackground(SkillType type) {
        foreach (var background in dictionaryOfSkillBackgrounds) {
            background.Value.gameObject.SetActive(background.Key == type);
        }
    }

    void ChangeDescriptionText(string text, string name) {
        skillLongDescription.text = text;
        skillName.text = name;
    }

    #endregion


    #endregion

}
