using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    [Foldout("Dictionary"), SerializedDictionary("Boss Fog", " Fog object"), SerializeField]
    SerializedDictionary<Bosses, GameObject> DictionaryOfFogs = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, GameObject> DictionaryOfButtons = new();
    [Foldout("Dictionary"), SerializedDictionary("Boss phase", " Boss Description"), SerializeField]
    SerializedDictionary<Bosses, BossDescription> DictinaryOfDescritions = new();

    [Foldout("First Map"), SerializeField] Button CloseMapButton;
    [Foldout("First Map"), SerializeField] Button TestIslandButton;
    [Foldout("First Map"), SerializeField] BossDescription TestIslandDescription;

    [Foldout("Second Map"), SerializeField] GameObject SecondMap;
    [Foldout("Second Map"), SerializeField] Image BossImage;
    [Foldout("Second Map"), SerializeField] Image SelectedCharacterIcon;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossName;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI IsleName;
    [Foldout("Second Map"), SerializeField] TextMeshProUGUI BossDescription;
    [Foldout("Second Map"), SerializeField] Button CloseSecondMapButton;
    [Foldout("Second Map"), SerializeField] Button SailButton;
    [Foldout("Second Map"), SerializeField] Button ChangeCharacterButton;
    [Foldout("Second Map"), SerializeField] List<Button> ListOfDifficultyButtons;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyBackgrounds;
    [Foldout("Second Map"), SerializeField] List<Image> ListOfDifficultyLocks;
    [Foldout("Second Map"), SerializeField] Color difficultySelectedColor;
    [Foldout("Second Map"), SerializeField] Color difficultyDeselectedColor;

    int _currentDifficulty = 0;

    CharacterSelectionManager _characterSelectionManager;

    public event Action OnCloseMap;

    Action<CharacterSO> _onChangeSelectedCharacter;

    #region StartRegion
    private void Awake() {
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
    private void OnEnable() {
        WhiteBoard board = WhiteBoard.Instance;

        foreach (var boss in board.ReturnListOfUnlockedPhasesByBoss().Keys) { // FOGS
            if (DictionaryOfFogs.ContainsKey(boss)) {
                DictionaryOfFogs[boss].SetActive(false);
            }
        }

        foreach (var phase in DictionaryOfButtons.Keys) { // ILHAS

            Button button = DictionaryOfButtons[phase].GetComponent<Button>();
            button.interactable = board.ReturnListOfUnlockedPhasesByBoss().ContainsKey(phase);
            ;

        }

        OnChangeSelectedCharacter(CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO());
    }
    #endregion

    void OnChangeSelectedCharacter(CharacterSO newCharacter) { SelectedCharacterIcon.sprite = newCharacter.CharacterIcon; }
    void SetButtons() {
        foreach (var pair in DictionaryOfButtons) {  // ILHAS
            if (!DictinaryOfDescritions.TryGetValue(pair.Key, out var description)) continue;

            Button button = pair.Value.GetComponent<Button>();
            var localDescription = description;

            button.onClick.AddListener(() => TurnScreenOn(localDescription));
        }

        for (int i = 0; i < ListOfDifficultyButtons.Count; i++) {
            int index = i;
            ListOfDifficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }

        CloseMapButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
            OnCloseMap?.Invoke();
            SecondMap.SetActive(false);
            SailButton.gameObject.SetActive(false);
            });
        
        CloseSecondMapButton.onClick.AddListener(() => {
            SecondMap.SetActive(false);
            SailButton.gameObject.SetActive(false);
        });

        TestIslandButton.onClick.AddListener(() => TurnScreenOn(TestIslandDescription));

        ChangeCharacterButton.onClick.AddListener(() => _characterSelectionManager.Initialize());
    }

    void SelectDifficulty(int difficulty) {
        _currentDifficulty = difficulty;

        foreach (var obj in ListOfDifficultyBackgrounds) {
            if (obj == ListOfDifficultyBackgrounds[difficulty]) {
                obj.color = difficultySelectedColor;
            }
            else obj.color = difficultyDeselectedColor;
        }
    }

    void TurnScreenOn(BossDescription description) {
        SecondMap.SetActive(true);
        BossImage.sprite = description.BossSprite;
        BossName.text = description.BossName;
        IsleName.text = description.IsleName;
        BossDescription.text = description.Description;

        SailButton.onClick.RemoveAllListeners();
        SailButton.onClick.AddListener(() => Sail(description));
        SailButton.gameObject.SetActive(true);

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
    void Sail(BossDescription description) {
        LoadingScreenManager.Instance.LoadFightScene(description.LoadingScreen[_currentDifficulty], description.ListOfScenes[_currentDifficulty]);
        OnCloseMap?.Invoke();
    }
}
