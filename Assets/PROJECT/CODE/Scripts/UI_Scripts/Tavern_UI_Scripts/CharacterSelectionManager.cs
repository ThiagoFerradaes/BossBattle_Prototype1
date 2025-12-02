using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour {
    [Header("Componentes")]
    [SerializeField] GameObject characterSelectionScreen;
    [SerializeField] Image selectedCharacterImage;
    [SerializeField] Image selectedCharacterSignature;
    [SerializeField] Image passiveIcon;
    [SerializeField] Image skillOneIcon;
    [SerializeField] Image skillTwoIcon;
    [SerializeField] Image ultimateIcon;
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

    #region StartRegion
    private void Start() {
        SetButtons();
    }

    void SetButtons() {
        foreach (var character in dictionaryOfCharactersButton.Keys) {
            dictionaryOfCharactersButton[character].onClick.AddListener(() => SelectCharacter(character));
        }

        closeScreenButton.onClick.AddListener(() => characterSelectionScreen.SetActive(false));
    }

    #endregion

    #region InitializeRegion

    public void Initialize() {

        ChangeSelectedImageAndSignature();
        ChangeSkillsIcon();
        LockIcons();

        characterSelectionScreen.SetActive(true);
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
        CharacterSO currentCharater = PlayerWhiteBoard.Instance.ReturnSelectedCharacterSO();

        selectedCharacterImage.sprite = currentCharater.CharacterSelectionImage;
        selectedCharacterSignature.sprite = currentCharater.CharacterSignature;

        foreach (var character in dictionaryOfBackgrounds.Keys) {
            bool isSelectedCharater = currentCharater.Character == character;
            dictionaryOfBackgrounds[character].color = isSelectedCharater ? selectedCharacterColor : unselectedCharacterColor;
        }
    }

    void ChangeSkillsIcon() {
        Character currentCharacter = PlayerWhiteBoard.Instance.ReturnSelectedCharacter();
        CommonSkillSO skillOne = PlayerWhiteBoard.Instance.ReturnSkillOne(currentCharacter);
        CommonSkillSO skillTwo = PlayerWhiteBoard.Instance.ReturnSkillTwo(currentCharacter);
        UltimateSkillSO ultimate = PlayerWhiteBoard.Instance.ReturnUltimate(currentCharacter);
        PassiveSO passive = PlayerWhiteBoard.Instance.ReturnPassive(currentCharacter);

        // Setando os icones
        passiveIcon.sprite = passive.PassiveIcon;
        skillOneIcon.sprite = skillOne.SkillSpriteIcon;
        skillTwoIcon.sprite = skillTwo.SkillSpriteIcon;
        ultimateIcon.sprite = ultimate.SkillSpriteIcon;

        // Setando as descrições
        passiveShortDescription.text = passive.ShortDescription;
        skillOneShortDescription.text = skillOne.SkillShortDescription;
        skillTwoShortDescription.text = skillTwo.SkillShortDescription;
        ultimateShortDescription.text = ultimate.SkillShortDescription;
    }
    #endregion

    #region InsideScreenMethods

    void SelectCharacter(CharacterSO character) {
        PlayerWhiteBoard.Instance.SetSelectedCharacter(character);
        ChangeSelectedImageAndSignature();
        ChangeSkillsIcon();
    }

    #endregion
}
