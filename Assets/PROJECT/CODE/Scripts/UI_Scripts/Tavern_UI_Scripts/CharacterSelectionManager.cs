using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour {
    [Header("Componentes")]
    [SerializeField] GameObject characterSelectionScreen;
    [SerializeField] Button characterSelectionBackground;
    [SerializeField] Button characterSelectionMask;
    [SerializeField] Image selectedCharacterImage;
    [SerializeField] Image selectedCharacterSignature;
    [SerializeField] Image selectedCharacterBackgroundImage;
    [SerializeField] TextMeshProUGUI selectedCharacterName;
    [SerializeField] Button closeScreenButton;

    [Header("Animations")]
    [SerializeField] Animator anim;
    [SerializeField] string enterAnimation;
    [SerializeField] string exitAnimation;

    [Header("Descriptions")]
    [SerializeField] TextMeshProUGUI passiveDescription;
    [SerializeField] TextMeshProUGUI skillOneShortDescription;
    [SerializeField] TextMeshProUGUI skillTwoShortDescription;
    [SerializeField] TextMeshProUGUI ultimateShortDescription;

    [SerializedDictionary("Character", "Button"), SerializeField]
    SerializedDictionary<CharacterSO, Button> dictionaryOfCharactersButton = new();

    [Header("Skills Icons")]
    [SerializeField] Image skillOneIcon;
    [SerializeField] Image skillTwoIcon;
    [SerializeField] Image ultimateIcon;
    [SerializedDictionary("Slot", "Button"), SerializeField] SerializedDictionary<SkillSlot, Button> dictionaryOfSkillSelectionButton;
    [SerializedDictionary("Slot", "Buttons"), SerializeField] SerializedDictionary<SkillSlot, List<Button>> dictionaryOfArrows;
    [SerializeField]SkillSelectionManager skillSelectionManager;
    List<CharacterUnlockedInfo> _unlockedInfo = new();

    #region StartRegion

    private void Start() {
        SetButtons();
    }

    void SetButtons() {
        // Bot�es de personagem para troca do personagem selecionado
        foreach (var character in dictionaryOfCharactersButton.Keys) {
            dictionaryOfCharactersButton[character].onClick.AddListener(() => SelectCharacter(character));
        }

        // Bot�o que fecha a UI;
        closeScreenButton.onClick.AddListener(() => {
            TurnScreenOff();
        });

        // Bot�o que abre a UI de sele��o de skill
        foreach (var slot in dictionaryOfSkillSelectionButton.Keys) {
            var tempSlot = slot;
            dictionaryOfSkillSelectionButton[tempSlot].onClick.AddListener(() => {
                skillSelectionManager.Initialize(tempSlot);
                closeScreenButton.gameObject.SetActive(false);
            });
        }

        characterSelectionBackground.onClick.AddListener(() => { skillSelectionManager.TurnScreenOff(); });

        characterSelectionMask.onClick.AddListener(ClosedSkillsUi);
    }

    #endregion

    #region InitializeRegion

    public void Initialize() {

        _unlockedInfo = WhiteBoard.Instance.ReturnListOfUnlockedCharacters();

        ChangeSelectedCharactersImages();
        ChangeAllSkillsIcon();
        ActivateCharacterSelectionButtons();

        characterSelectionScreen.SetActive(true);

        //if (anim.gameObject.activeInHierarchy) anim.Play(enterAnimation);

    }

    public void TurnScreenOff() {
        characterSelectionScreen.SetActive(false);
        skillSelectionManager.TurnScreenOff();

        //if (anim.gameObject.activeInHierarchy) anim.Play(exitAnimation);
    }

    void ChangeSelectedCharactersImages() {

        // Pegando o personagem atual
        CharacterSO currentCharater = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO();

        // Trocando a splashArt
        selectedCharacterImage.sprite = currentCharater.CharacterSelectionImage;

        // Trocando assinatura
        selectedCharacterSignature.sprite = currentCharater.CharacterSignature;

        // Trocando o background por trás das habilidades
        selectedCharacterBackgroundImage.sprite = currentCharater.CharacterSelectedBackground;

        // Trocando o nome
        selectedCharacterName.text = currentCharater.CharacterName.GetLocalizedString();

        // Trocando a imagem do botão do personagem selecionado
        foreach (var info in _unlockedInfo) {
            var character = info.Character;
            if (character.Character == Character.Julian) continue;
            else if (info.Character == currentCharater)
                dictionaryOfCharactersButton[character].GetComponent<Image>().sprite = character.SelectedCharacterMapSprite;
            else if (info.IsUnlocked)
                dictionaryOfCharactersButton[character].GetComponent<Image>().sprite = character.UnselectedCharacterMapSprite;
            else
                dictionaryOfCharactersButton[character].GetComponent<Image>().sprite = character.CharacterLockedMapSprite;
        }

    }

    void ChangeAllSkillsIcon() { 

        ChangeSkillIcon(SkillSlot.SkillOne);
        ChangeSkillIcon(SkillSlot.SkillTwo);
        ChangeSkillIcon(SkillSlot.Ultimate);

        HandleArrows();

        ChangePassiveText();
    }
    public void ChangeSkillIcon(SkillSlot slot) {
        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(slot);
        switch (slot) {
            case SkillSlot.SkillOne:
                skillOneIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                skillOneShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription.GetLocalizedString();
                break;
            case SkillSlot.SkillTwo:
                skillTwoIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                skillTwoShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription.GetLocalizedString();
                break;
            case SkillSlot.Ultimate:
                ultimateIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                ultimateShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription.GetLocalizedString();
                break;
        }
    }

    void ChangePassiveText() {
        PassiveSO passive = CurrentSelectedCharacterWhiteBoard.Instance.ReturnPassive();
        passiveDescription.text = passive.ShortDescription.GetLocalizedString();
    }
    void HandleArrows() {
        Character currentCharacter = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();

        // Setando as setas
        CharacterUnlockedInfo currentCharacterUnlockInfo = _unlockedInfo.Where(p => p.Character.Character == currentCharacter).FirstOrDefault();
        foreach (var slot in dictionaryOfArrows.Keys) {

            List<SkillUnlockedInfo> skillsInfo = currentCharacterUnlockInfo.DictionaryOfUnlockedSkills[slot];
            SkillUnlockedInfo alternativeSkill = skillsInfo.Where(p => p.Type == SkillType.Alternative).FirstOrDefault();

            foreach (var arrow in dictionaryOfArrows[slot]) {

                bool isUnlocked = alternativeSkill.IsUnlocked;

                if (isUnlocked) arrow.onClick.AddListener(() => ChangeCurrentSkillInDisplay(slot, skillsInfo));
                else arrow.onClick.RemoveAllListeners();

                arrow.gameObject.SetActive(alternativeSkill.IsUnlocked);
            }
        }
    }
    void ChangeCurrentSkillInDisplay(SkillSlot slot, List<SkillUnlockedInfo> skillsInfo) {

        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(slot);
        SkillType type = currentSkill.SkillType;
        SkillSO newSkill = skillsInfo.Where(p => p.Type != type).FirstOrDefault().Skill;

        CurrentSelectedCharacterWhiteBoard.Instance.SetCurrentCharacterSkillBySlot(slot, newSkill);

        ChangeSkillIcon(slot);
    }



    void ActivateCharacterSelectionButtons() {
        foreach (var info in _unlockedInfo) {
            if (info.Character.Character == Character.Julian) continue;
            dictionaryOfCharactersButton[info.Character].interactable = info.IsUnlocked;
        }
    }

    public void ClosedSkillsUi() {
        skillSelectionManager.TurnScreenOff();
        characterSelectionScreen.SetActive(false);
    }

    public void TurnCloseButtonOn() {
        closeScreenButton.gameObject.SetActive(true);
    }

    #endregion

    #region InsideScreenMethods

    void SelectCharacter(CharacterSO character) {
        CurrentSelectedCharacterWhiteBoard.Instance.SetSelectedCharacter(character);
        ChangeSelectedCharactersImages();
        ChangeAllSkillsIcon();
        skillSelectionManager.TurnScreenOff();
    }
    #endregion
}
