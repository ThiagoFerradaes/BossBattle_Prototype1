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
    [SerializeField] Color unselectedSkillColor;
    [SerializeField] Color selectedSkillColor;
    [SerializedDictionary("Slot", "Button"), SerializeField] SerializedDictionary<SkillSlot, Image> dictionaryOfSkillsIconBackground;
    [SerializedDictionary("Slot", "Button"), SerializeField] SerializedDictionary<SkillSlot, Button> dictionaryOfSkillSelectionButton;
    SkillSelectionManager _skillSelectionManager;
    List<CharacterUnlockedInfo> _unlockedInfo = new();

    private bool isInitialized;

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

        isInitialized = true;
    }

    #endregion

    #region InitializeRegion

    public async void Initialize() {
        try {
            bool set = false;
            while (!isInitialized) {
                if (set) {
                    await Task.Yield();
                    continue;
                }
                SetButtons();
                set = true;
            }

            _unlockedInfo = WhiteBoard.Instance.ReturnListOfUnlockedCharecters();

            ChangeSelectedCharactersImages();
            ChangeSkillsIcon();
            ActivateCharacterSelectionButtons();

            characterSelectionScreen.SetActive(true);

        }
        catch {
            // ignore
        }
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
            if (info.Character == currentCharater)
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
        skillOneIcon.sprite = skillOne.MapSkillSpriteIcon;
        skillTwoIcon.sprite = skillTwo.MapSkillSpriteIcon;
        ultimateIcon.sprite = ultimate.MapSkillSpriteIcon;

        // Setando as descri��es
        passiveShortDescription.text = passive.ShortDescription;
        skillOneShortDescription.text = skillOne.SkillShortDescription;
        skillTwoShortDescription.text = skillTwo.SkillShortDescription;
        ultimateShortDescription.text = ultimate.SkillShortDescription;
    }

    public void ChangeSkillIcon(SkillSlot slot) {
        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(slot);
        switch (slot) {
            case SkillSlot.SkillOne:
                skillOneIcon.sprite = currentSkill.UISkillSpriteIcon;
                skillOneShortDescription.text = currentSkill.SkillShortDescription;
                break;
            case SkillSlot.SkillTwo:
                skillTwoIcon.sprite = currentSkill.UISkillSpriteIcon;
                skillTwoShortDescription.text = currentSkill.SkillShortDescription;
                break;
            case SkillSlot.Ultimate:
                ultimateIcon.sprite = currentSkill.UISkillSpriteIcon;
                ultimateShortDescription.text = currentSkill.SkillShortDescription;
                break;
        }
    }

    void ActivateCharacterSelectionButtons() {
        foreach (var info in _unlockedInfo) {
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
            if (skillIcon.Key == activeSlot) skillIcon.Value.color = selectedSkillColor;
            else skillIcon.Value.color = unselectedSkillColor;
        }
    }

    public void EraseSkillIconBackgroundSelection() {
        foreach (var skillIcon in dictionaryOfSkillsIconBackground) {
            skillIcon.Value.color = unselectedSkillColor;
        }
    }
    #endregion
}
