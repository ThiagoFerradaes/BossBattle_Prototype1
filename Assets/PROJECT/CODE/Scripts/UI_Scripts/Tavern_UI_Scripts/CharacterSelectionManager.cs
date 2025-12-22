using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CharacterSelectionManager : MonoBehaviour {
    [Header("Componentes")]
    [SerializeField] GameObject characterSelectionScreen;
    [SerializeField] Button characterSelectionBackground;
    [SerializeField] Button characterSelectionMask;
    [SerializeField] Image selectedCharacterImage;
    [SerializeField] Image selectedCharacterSignature;
    [SerializeField] TextMeshProUGUI passiveShortDescription;
    [SerializeField] TextMeshProUGUI skillOneShortDescription;
    [SerializeField] TextMeshProUGUI skillTwoShortDescription;
    [SerializeField] TextMeshProUGUI ultimateShortDescription;
    [SerializeField] Button closeScreenButton;
    [SerializeField] Color unselectedCharacterColor;
    [SerializeField] Color selectedCharacterColor;
    [SerializedDictionary("Character", "Button"), SerializeField]
    SerializedDictionary<CharacterSO, Button> dictionaryOfCharactersButton = new();
    [SerializedDictionary("Character", "Image"), SerializeField]
    SerializedDictionary<Character, Image> dictionaryOfLocks = new();
    [SerializedDictionary("Character", "Image"), SerializeField]
    SerializedDictionary<Character, Image> dictionaryOfBackgrounds = new();

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

    public async void Initialize()
    {
        try
        {
            bool set = false; 
            while(!isInitialized)
            {
                if (set)
                {
                    await Task.Yield();
                    continue;
                }
                SetButtons();
                set = true;
            }
        
            ChangeSelectedImageAndSignature();
            ChangeSkillsIcon();
            LockIcons();

            characterSelectionScreen.SetActive(true);
        }
        catch
        {
            // ignore
        }
    }

    void LockIcons() {
        List<CharacterUnlockedInfo> listOfUnlockedCharactersInfo = WhiteBoard.Instance.ReturnListOfUnlockedCharecters();

        foreach (CharacterUnlockedInfo character in listOfUnlockedCharactersInfo) {
            dictionaryOfLocks[character.Character.Character].gameObject.SetActive(!character.IsUnlocked);
        }

        ActivateCharacterSelectionButtons(listOfUnlockedCharactersInfo);
    }

    void ActivateCharacterSelectionButtons(List<CharacterUnlockedInfo> listOfUnlockedCharacters) {
        foreach (var info in listOfUnlockedCharacters) {
            dictionaryOfCharactersButton[info.Character].interactable = info.IsUnlocked;
        }
    }

    void ChangeSelectedImageAndSignature() {
        CharacterSO currentCharater = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO();

        selectedCharacterImage.sprite = currentCharater.CharacterSelectionImage;
        selectedCharacterSignature.sprite = currentCharater.CharacterSignature;

        foreach (var character in dictionaryOfBackgrounds.Keys) {
            bool isSelectedCharater = currentCharater.Character == character;
            dictionaryOfBackgrounds[character].color = isSelectedCharater ? selectedCharacterColor : unselectedCharacterColor;
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
        skillOneIcon.sprite = skillOne.SkillSpriteIcon;
        skillTwoIcon.sprite = skillTwo.SkillSpriteIcon;
        ultimateIcon.sprite = ultimate.SkillSpriteIcon;

        // Setando as descri��es
        passiveShortDescription.text = passive.ShortDescription;
        skillOneShortDescription.text = skillOne.SkillShortDescription;
        skillTwoShortDescription.text = skillTwo.SkillShortDescription;
        ultimateShortDescription.text = ultimate.SkillShortDescription;
    }

    public void ClosedSkillsUi()
    {
        _skillSelectionManager.TurnScreenOff();
        characterSelectionScreen.SetActive(false);
    }
    
    public void ChangeSkillIcon(SkillSlot slot) {
        SkillSO currentSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(slot);
        switch (slot) {
            case SkillSlot.SkillOne:
                skillOneIcon.sprite = currentSkill.SkillSpriteIcon;
                skillOneShortDescription.text = currentSkill.SkillShortDescription;
                break;
            case SkillSlot.SkillTwo:
                skillTwoIcon.sprite = currentSkill.SkillSpriteIcon;
                skillTwoShortDescription.text = currentSkill.SkillShortDescription;
                break;
            case SkillSlot.Ultimate:
                ultimateIcon.sprite = currentSkill.SkillSpriteIcon;
                ultimateShortDescription.text = currentSkill.SkillShortDescription;
                break;
        }
    }
    #endregion

    #region InsideScreenMethods

    void SelectCharacter(CharacterSO character) {
        CurrentSelectedCharacterWhiteBoard.Instance.SetSelectedCharacter(character);
        ChangeSelectedImageAndSignature();
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
