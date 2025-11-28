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
    [SerializedDictionary("Button", "Character"), SerializeField]
    SerializedDictionary<Button, CharacterSO> dictionaryOfCharactersButton = new();
    [SerializedDictionary("Character", "Image"), SerializeField]
    SerializedDictionary<Character, Image> dictionaryOfLocks = new();
    [SerializedDictionary("Character", "Image"), SerializeField]
    SerializedDictionary<Character, Image> dictionaryOfBackgrounds = new();

    #region StartRegion
    private void Start() {
        SetButtons();
    }

    void SetButtons() {
        foreach (var button in dictionaryOfCharactersButton.Keys) {
            var charater = dictionaryOfCharactersButton[button];
            button.onClick.AddListener(() => SelectCharacter(charater));
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
        List<Character> listOfUnlockedCharacters = WhiteBoard.Instance.ReturnListOfUnlockedCharecters();

        foreach (Character character in dictionaryOfLocks.Keys) {
            bool isUnlocked = listOfUnlockedCharacters.Contains(character);
            dictionaryOfLocks[character].gameObject.SetActive(!isUnlocked);
        }

        ActivateCharacterSelectionButtons(listOfUnlockedCharacters);
    }

    void ActivateCharacterSelectionButtons(List<Character> listOfUnlockedCharacters) {
        foreach (Button button in dictionaryOfCharactersButton.Keys) {
            CharacterSO character = dictionaryOfCharactersButton[button];

            bool characterIsUnlocked = listOfUnlockedCharacters.Contains(character.Character);
            button.interactable = characterIsUnlocked;

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
