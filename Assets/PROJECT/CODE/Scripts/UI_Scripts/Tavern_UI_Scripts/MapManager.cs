using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    [SerializeField] GameObject mapScreen;

    [Foldout("Dictionary"), SerializedDictionary("Boss Fog", " Fog object"), SerializeField]
    SerializedDictionary<Bosses, GameObject> DictionaryOfFogs = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, Button> DictionaryOfButtons = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, Sprite> DictionaryOfSelectedSprites = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, Sprite> DictionaryOfUnselectedSprites = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, Sprite> DictionaryOfHoverSprites = new();
    [Foldout("Dictionary"), SerializedDictionary("Boss phase", " Boss Description"), SerializeField]
    SerializedDictionary<Bosses, BossDescription> DictinaryOfDescritions = new();

    Button _selectedIslandButton;

    [Foldout("First Map"), SerializeField] Button CloseMapButton;
    [Foldout("First Map"), SerializeField] Button TestIslandButton;
    [Foldout("First Map"), SerializeField] BossDescription TestIslandDescription;
    [Foldout("First Map"), SerializeField] GameObject selectedIslandIcon;
    [Foldout("First Map"), SerializeField] Sprite normalSailButtonSprite;
    [Foldout("First Map"), SerializeField] Sprite enterSailButtonSprite;
    [Foldout("First Map"), SerializeField] Sprite enterExitButtonSprite;
    [Foldout("First Map"), SerializeField] Sprite exitExitButtonSprite;

    [Foldout("Second Map"), SerializeField] GameObject SecondMap;
    [Foldout("Second Map"), SerializeField] Image BossImage;
    [Foldout("Second Map"), SerializeField] Image SelectedCharacterIcon;
    [Foldout("Second Map"), SerializeField] LocalizeSpriteEvent IsleName;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossDescription;
    [Foldout("Second Map"), SerializeField] Button CloseSecondMapButton;
    [Foldout("Second Map"), SerializeField] Button SailButton;
    [Foldout("Second Map"), SerializeField] Button ChangeCharacterButton;
    [Foldout("Second Map"), SerializeField] List<Sprite> listOfDificultySpritesActive;
    [Foldout("Second Map"), SerializeField] List<Sprite> listOfDificultySpritesDesactive;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyImages;
    [Foldout("Second Map"), SerializeField] List<Button> ListOfDifficultyButtons;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyLocks;
    [Foldout("Second Map"), SerializeField] Color difficultySelectedColor;
    [Foldout("Second Map"), SerializeField] Color difficultyDeselectedColor;

    int _currentDifficulty = 0;

    CharacterSelectionManager _characterSelectionManager;

    public event Action OnCloseMap;

    Action<CharacterSO> _onChangeSelectedCharacter;

    PlayerInputHandlerManager _handler;


    #region StartRegion
    private void Awake() {
        TurnMapOff();

        _characterSelectionManager = GetComponent<CharacterSelectionManager>();
        _onChangeSelectedCharacter = OnChangeSelectedCharacter;

        SetButtons();
    }
    private void Start() {
        CurrentSelectedCharacterWhiteBoard.Instance.OnSelectedCharacterChanged += _onChangeSelectedCharacter;
    }
    private void OnDestroy() {
        CurrentSelectedCharacterWhiteBoard.Instance.OnSelectedCharacterChanged -= _onChangeSelectedCharacter;
    }
    public void InitializeMap(PlayerInputHandlerManager handler = null) {
        _handler = handler;

        WhiteBoard board = WhiteBoard.Instance;

        foreach (var boss in board.ReturnListOfUnlockedPhasesByBoss().Keys) { // FOGS
            if (DictionaryOfFogs.ContainsKey(boss)) {
                DictionaryOfFogs[boss].SetActive(false);
            }
        }

        foreach (var phase in DictionaryOfButtons.Keys) { // ILHAS

            Button button = DictionaryOfButtons[phase].GetComponent<Button>();
            button.interactable = board.ReturnListOfUnlockedPhasesByBoss().ContainsKey(phase);


        }

        OnChangeSelectedCharacter(CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO());

        mapScreen.SetActive(true);
    }
    #endregion

    void OnChangeSelectedCharacter(CharacterSO newCharacter) { SelectedCharacterIcon.sprite = newCharacter.CharacterIcon; }
    void SetButtons() {
        foreach (var pair in DictionaryOfButtons) {  // ILHAS
            if (!DictinaryOfDescritions.TryGetValue(pair.Key, out var description)) continue;

            Button button = pair.Value;
            var localDescription = description;

            Bosses boss = pair.Key;

            button.onClick.AddListener(() => TurnBossDiffucltyScreenOn(localDescription));
            button.onClick.AddListener(() => TurnIslandSelectSpriteOn(boss));

        }

        for (int i = 0; i < ListOfDifficultyButtons.Count; i++) {
            int index = i;
            ListOfDifficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }

        CloseMapButton.onClick.AddListener(() => {
            TurnMapOff();
            OnCloseMap?.Invoke();
            SecondMap.SetActive(false);
            SailButton.gameObject.SetActive(false);
        });

        CloseSecondMapButton.onClick.AddListener(() => {
            TurnDifficultyScreenOf();
            TurnAllIslandSelectSpriteOff();
        });

        TestIslandButton.onClick.AddListener(() => TurnBossDiffucltyScreenOn(TestIslandDescription));

        ChangeCharacterButton.onClick.AddListener(() => _characterSelectionManager.Initialize());
    }

    void TurnMapOff() {
        if (_handler != null) _handler.SetCanInput(true);
        mapScreen.SetActive(false);
    }
    void TurnDifficultyScreenOf() {

        SecondMap.SetActive(false);
        SailButton.gameObject.SetActive(false);
        CloseMapButton.gameObject.SetActive(true);
    }

    #region Chang Island Sprite Methods
    void TurnIslandSelectSpriteOn(Bosses bossRelatedToTheIsland) {
        foreach (var pair in DictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Key == bossRelatedToTheIsland) {
                image.sprite = DictionaryOfSelectedSprites[pair.Key];
                _selectedIslandButton = pair.Value;
            }
            else image.sprite = DictionaryOfUnselectedSprites[pair.Key];
        }
    }
    void TurnAllIslandSelectSpriteOff() {
        foreach (var pair in DictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            image.sprite = DictionaryOfUnselectedSprites[pair.Key];
        }

        _selectedIslandButton = null;
    }
    public void TurnIslandHooverSpriteOn(Button bossRelatedToTheIsland) {

        if (!DictionaryOfButtons.ContainsValue(bossRelatedToTheIsland) || _selectedIslandButton == bossRelatedToTheIsland || !bossRelatedToTheIsland.interactable) return;

        foreach (var pair in DictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Value == bossRelatedToTheIsland) image.sprite = DictionaryOfHoverSprites[pair.Key];
        }
    }
    public void TurnIslandSelectSpriteOff(Button bossRelatedToTheIsland) {

        if (!DictionaryOfButtons.ContainsValue(bossRelatedToTheIsland) || _selectedIslandButton == bossRelatedToTheIsland || !bossRelatedToTheIsland.interactable) return;

        foreach (var pair in DictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Value == bossRelatedToTheIsland) image.sprite = DictionaryOfUnselectedSprites[pair.Key];
        }
    }

    #endregion

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

    void TurnBossDiffucltyScreenOn(BossDescription description) {

        // Limpando objetos antigos
        CloseMapButton.gameObject.SetActive(false);

        // Mudando as informações da tela de dificuldade do boss
        BossImage.sprite = description.BossSprite;
        IsleName.AssetReference = description.IsleName;
        BossDescription.text = description.Description.GetLocalizedString();
        SecondMap.SetActive(true);

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

    }


    #region Button Methods
    void Sail(BossDescription description) {
        LoadingScreenManager.CurrentLoadingScreenInfo = description.LoadingScreen[_currentDifficulty];
        OnCloseMap?.Invoke();
        SceneManager.LoadScene(1);
    }
    public void EnterSailButton(Image sailImage) {
        sailImage.sprite = enterSailButtonSprite;
    }
    public void ExitSailButton(Image sailImage) {
        sailImage.sprite = normalSailButtonSprite;
    }
    public void EnterExitButton(Image exitImage) {
        exitImage.sprite = enterExitButtonSprite;
    }
    public void ExitExitButton(Image exitImage) {
        exitImage.sprite = exitExitButtonSprite;
    }
    #endregion
}
