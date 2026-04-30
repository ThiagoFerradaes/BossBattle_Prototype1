using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossDifficultyManager : MonoBehaviour
{
    [Foldout("Components") ,SerializeField] GameObject BossDifficultyScreen;
    [Foldout("Components") ,SerializeField] CharacterSelectionManager characterSelectionManager;
    [Foldout("Components"), SerializeField] MapManager mapManager;
    [Foldout("Components"), SerializeField] Image BossImage;
    [Foldout("Components"), SerializeField] Image SelectedCharacterIcon;
    [Foldout("Components"), SerializeField] LocalizeSpriteEvent IsleName;
    [Foldout("Components"), SerializeField] TextMeshProUGUI BossDescription;
    [Foldout("Components"), SerializeField] Button CloseButton;
    [Foldout("Components"), SerializeField] Button SailButton;
    [Foldout("Components"), SerializeField] Button ChangeCharacterButton;
    [Foldout("Components"), SerializeField] LocalizeSpriteEvent ChangeCharacterButtonLocalizeEvent;

    [Foldout("List"), SerializeField] List<Sprite> listOfDificultySpritesActive;
    [Foldout("List"), SerializeField] List<Sprite> listOfDificultySpritesDesactive;
    [Foldout("List"), SerializeField] List<Image> ListOfDifficultyImages;
    [Foldout("List"), SerializeField] List<Button> ListOfDifficultyButtons;
    [Foldout("List"), SerializeField] List<Image> ListOfDifficultyLocks;

    [Foldout("Sprites"), SerializeField] Sprite normalSailButtonSprite;
    [Foldout("Sprites"), SerializeField] Sprite enterSailButtonSprite;
    [Foldout("Sprites"), SerializeField] LocalizedSprite CharacterSelectionSelectedLocalizedSprite;
    [Foldout("Sprites"), SerializeField] LocalizedSprite CharacterSelectionUnselectedLocalizedSprite;

    int _currentDifficulty = 0;

    public event Action OnCloseMap;
    Action<CharacterSO> _onChangeSelectedCharacter;

    private void Awake() {
        _onChangeSelectedCharacter = OnChangeSelectedCharacter; 
        SetButtons();
    }
    private void Start() {
        CurrentSelectedCharacterWhiteBoard.Instance.OnSelectedCharacterChanged += _onChangeSelectedCharacter;
    }
    private void OnDestroy() {
        CurrentSelectedCharacterWhiteBoard.Instance.OnSelectedCharacterChanged -= _onChangeSelectedCharacter;
    }
    void SetButtons() {
        for (int i = 0; i < ListOfDifficultyButtons.Count; i++) {
            int index = i;
            ListOfDifficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }

        CloseButton.onClick.AddListener(() => {
            TurnBossDifficultyScreenOff();
        });

        ChangeCharacterButton.onClick.AddListener(() => characterSelectionManager.Initialize());
    }

    void OnChangeSelectedCharacter(CharacterSO newCharacter) { SelectedCharacterIcon.sprite = newCharacter.UnselectedCharacterMapSprite; }

    public void TurnBossDifficultyScreenOn(BossDescription description) {
        // Mudando as informações da tela de dificuldade do boss
        BossImage.sprite = description.BossSprite;
        IsleName.AssetReference = description.IsleName;
        BossDescription.text = description.Description.GetLocalizedString();
        BossDifficultyScreen.SetActive(true);

        // Setando o botão de Sail
        SailButton.onClick.RemoveAllListeners();
        SailButton.onClick.AddListener(() => Sail(description));
        SailButton.gameObject.SetActive(true);

        // Setando as dificuldades disponiveis
        int amountOfPhasesUnlocked = WhiteBoard.Instance.ReturnListOfUnlockedPhasesByBoss()[description.Boss];

        for (int i = 0; i < ListOfDifficultyLocks.Count; i++) {
            if (i <= amountOfPhasesUnlocked) {
                ListOfDifficultyLocks[i].gameObject.SetActive(false);
                ListOfDifficultyButtons[i].interactable = true;
            }
            else {
                ListOfDifficultyLocks[i].gameObject.SetActive(true);
                ListOfDifficultyButtons[i].interactable = false;
            }
        }

        SelectDifficulty(0);

        OnChangeSelectedCharacter(CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO());
    }

    public void TurnBossDifficultyScreenOff() {

        BossDifficultyScreen.SetActive(false);

        SailButton.gameObject.SetActive(false);

        mapManager.TurnDifficultyScreenOff(false);
    }

    void Sail(BossDescription description) {
        LoadingScreenManager.CurrentLoadingScreenInfo = description.LoadingScreen[_currentDifficulty];
        OnCloseMap?.Invoke();
        SceneManager.LoadScene(1);
    }

    void SelectDifficulty(int difficulty) {
        _currentDifficulty = difficulty;

        for (int i = 0; i < listOfDificultySpritesActive.Count; i++) {
            if (i <= difficulty) {
                ListOfDifficultyImages[i].sprite = listOfDificultySpritesActive[i];
            }
            else {
                ListOfDifficultyImages[i].sprite = listOfDificultySpritesDesactive[i];
            }
        }
    }
    public void EnterSailButton(Image sailImage) {
        sailImage.sprite = enterSailButtonSprite;
    }
    public void ExitSailButton(Image sailImage) {
        sailImage.sprite = normalSailButtonSprite;
    }

    public void EnterCharacterSelectionButton() {
        ChangeCharacterButtonLocalizeEvent.AssetReference = CharacterSelectionSelectedLocalizedSprite;
    }

    public void ExitCharacterSelectionButton() {
        ChangeCharacterButtonLocalizeEvent.AssetReference = CharacterSelectionUnselectedLocalizedSprite;
    }
}
