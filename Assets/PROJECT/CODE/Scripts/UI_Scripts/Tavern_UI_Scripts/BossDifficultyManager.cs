using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossDifficultyManager : MonoBehaviour
{
    [SerializeField] GameObject BossDifficultyScreen;
    [SerializeField] CharacterSelectionManager characterSelectionManager;

    [Foldout("Second Map"), SerializeField] Image BossImage;
    [Foldout("Second Map"), SerializeField] Image SelectedCharacterIcon;
    [Foldout("Second Map"), SerializeField] LocalizeSpriteEvent IsleName;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossDescription;
    [Foldout("Second Map"), SerializeField] Button CloseButton;
    [Foldout("Second Map"), SerializeField] Button SailButton;
    [Foldout("Second Map"), SerializeField] Button ChangeCharacterButton;
    [Foldout("Second Map"), SerializeField] List<Sprite> listOfDificultySpritesActive;
    [Foldout("Second Map"), SerializeField] List<Sprite> listOfDificultySpritesDesactive;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyImages;
    [Foldout("Second Map"), SerializeField] List<Button> ListOfDifficultyButtons;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyLocks;

    [Foldout("First Map"), SerializeField] Sprite normalSailButtonSprite;
    [Foldout("First Map"), SerializeField] Sprite enterSailButtonSprite;

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

    void OnChangeSelectedCharacter(CharacterSO newCharacter) { SelectedCharacterIcon.sprite = newCharacter.CharacterIcon; }

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
}
