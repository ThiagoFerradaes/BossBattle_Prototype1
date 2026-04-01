using AYellowpaper.SerializedCollections;
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
    [SerializeField] TextMeshProUGUI passiveShortDescription;
    [SerializeField] TextMeshProUGUI skillOneShortDescription;
    [SerializeField] TextMeshProUGUI skillTwoShortDescription;
    [SerializeField] TextMeshProUGUI ultimateShortDescription;
    [SerializeField] Button closeScreenButton;
    [SerializeField] Color unselectedCharacterColor;
    [SerializeField] Color selectedCharacterColor;
    [SerializedDictionary("Character", "Button"), SerializeField]
    SerializedDictionary<CharacterSO, Button> dictionaryOfCharactersButton = new();

    [Header("Skills Icons")]
    [SerializeField] Image passiveIcon;
    [SerializeField] Image skillOneIcon;
    [SerializeField] Image skillTwoIcon;
    [SerializeField] Image ultimateIcon;
    [SerializedDictionary("Slot", "Button"), SerializeField] SerializedDictionary<SkillSlot, Image> dictionaryOfSkillsIconBackground;
    [SerializedDictionary("Slot", "Button"), SerializeField] SerializedDictionary<SkillSlot, Button> dictionaryOfSkillSelectionButton;
    SkillSelectionManager _skillSelectionManager;
    List<CharacterUnlockedInfo> _unlockedInfo = new();

    #region StartRegion
    private void Awake() {
        _skillSelectionManager = GetComponent<SkillSelectionManager>();
    }
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
            characterSelectionScreen.SetActive(false);
            _skillSelectionManager.TurnScreenOff();
        });

        // Bot�o que abre a UI de sele��o de skill
        foreach (var slot in dictionaryOfSkillSelectionButton.Keys) {
            var tempSlot = slot;
            dictionaryOfSkillSelectionButton[tempSlot].onClick.AddListener(() => {
                _skillSelectionManager.Initialize(tempSlot);
                ChangeSkillIconBackground(tempSlot);
            });
        }

        characterSelectionBackground.onClick.AddListener(() => { _skillSelectionManager.TurnScreenOff(); });

        characterSelectionMask.onClick.AddListener(ClosedSkillsUi);
    }

    #endregion

    #region InitializeRegion

    public void Initialize() {

        _unlockedInfo = WhiteBoard.Instance.ReturnListOfUnlockedCharecters();

        ChangeSelectedCharactersImages();
        ChangeSkillsIcon();
        ActivateCharacterSelectionButtons();
        TurnOffSkillSelectionBackground();

        characterSelectionScreen.SetActive(true);

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

        // Trocando a imagem do botão do personagem selecionado
        foreach(var info in _unlockedInfo) {
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

    void ChangeSkillsIcon() {
        Character currentCharacter = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();
        CommonSkillSO skillOne = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillOne(currentCharacter);
        CommonSkillSO skillTwo = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSkillTwo(currentCharacter);
        UltimateSkillSO ultimate = CurrentSelectedCharacterWhiteBoard.Instance.ReturnUltimate(currentCharacter);
        PassiveSO passive = CurrentSelectedCharacterWhiteBoard.Instance.ReturnPassive(currentCharacter);

        // Setando os icones
        passiveIcon.sprite = passive.PassiveIcon;
        skillOneIcon.sprite = skillOne.MapDescriptionInfo.MapSkillSpriteIcon;
        skillTwoIcon.sprite = skillTwo.MapDescriptionInfo.MapSkillSpriteIcon;
        ultimateIcon.sprite = ultimate.MapDescriptionInfo.MapSkillSpriteIcon;

        // Setando as descri��es
        passiveShortDescription.text = passive.ShortDescription;
        skillOneShortDescription.text = skillOne.MapDescriptionInfo.SkillShortDescription;
        skillTwoShortDescription.text = skillTwo.MapDescriptionInfo.SkillShortDescription;
        ultimateShortDescription.text = ultimate.MapDescriptionInfo .SkillShortDescription;
    }

    public void ChangeSkillIcon(SkillSlot slot) {
        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(slot);
        switch (slot) {
            case SkillSlot.SkillOne:
                skillOneIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                skillOneShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription;
                break;
            case SkillSlot.SkillTwo:
                skillTwoIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                skillTwoShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription;
                break;
            case SkillSlot.Ultimate:
                ultimateIcon.sprite = currentSkill.MapDescriptionInfo.MapSkillSpriteIcon;
                ultimateShortDescription.text = currentSkill.MapDescriptionInfo.SkillShortDescription;
                break;
        }
    }

    void ActivateCharacterSelectionButtons() {
        foreach (var info in _unlockedInfo) {
            if (info.Character.Character == Character.Julian) continue;
            dictionaryOfCharactersButton[info.Character].interactable = info.IsUnlocked;
        }
    }

    public void ClosedSkillsUi() {
        _skillSelectionManager.TurnScreenOff();
        characterSelectionScreen.SetActive(false);
    }


    #endregion

    #region InsideScreenMethods

    void SelectCharacter(CharacterSO character) {
        CurrentSelectedCharacterWhiteBoard.Instance.SetSelectedCharacter(character);
        ChangeSelectedCharactersImages();
        ChangeSkillsIcon();
        _skillSelectionManager.TurnScreenOff();
    }

    void ChangeSkillIconBackground(SkillSlot activeSlot) {
        foreach (var skillIcon in dictionaryOfSkillsIconBackground) {
            skillIcon.Value.gameObject.SetActive(skillIcon.Key == activeSlot);
        }
    }

    public void TurnOffSkillSelectionBackground() {
        foreach (var skillIcon in dictionaryOfSkillsIconBackground) {
            skillIcon.Value.gameObject.SetActive(false);
        }
    }
    #endregion
}
