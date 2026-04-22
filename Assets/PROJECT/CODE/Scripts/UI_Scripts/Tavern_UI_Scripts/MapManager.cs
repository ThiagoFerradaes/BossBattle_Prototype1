using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    // Dicionarios de componentes
    [Foldout("Dictionary"), SerializedDictionary("Boss Fog", " Fog object"), SerializeField]
    SerializedDictionary<Bosses, GameObject> dictionaryOfFogs = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Button object"), SerializeField]
    SerializedDictionary<Bosses, Button> dictionaryOfButtons = new();
    [Foldout("Dictionary"), SerializedDictionary("Boss phase", " Boss Description"), SerializeField]
    SerializedDictionary<Bosses, BossDescription> dictinaryOfDescritions = new();

    // Dicionarios de sprites
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Sprites"), SerializeField]
    SerializedDictionary<Bosses, Sprite> dictionaryOfSelectedSprites = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Sprites"), SerializeField]
    SerializedDictionary<Bosses, Sprite> dictionaryOfUnselectedSprites = new();
    [Foldout("Dictionary"), SerializedDictionary("Button phase", " Sprites"), SerializeField]
    SerializedDictionary<Bosses, Sprite> dictionaryOfHoverSprites = new();


    [Foldout("Components"), SerializeField] GameObject mapScreen;
    [Foldout("Components"), SerializeField] Button closeMapButton;
    [Foldout("Components"), SerializeField] BossDifficultyManager bossDifficultyManager;
    [Foldout("Components"), SerializeField] CharacterSelectionManager characterSelectionManager;

    [Foldout("Sprites"), SerializeField] Sprite enterExitButtonSprite;
    [Foldout("Sprites"), SerializeField] Sprite exitExitButtonSprite;

    [Foldout("Temp"), SerializeField] Button testIslandButton;
    [Foldout("Temp"), SerializeField] BossDescription TestIslandDescription;


    Button _selectedIslandButton;
    PlayerInputHandlerManager _handler;


    #region StartRegion
    private void Awake() {

        TurnMapOff();
        SetButtons();
    }

    void TurnMapOff() {

        if (_handler != null) _handler.SetCanInput(true);

        // Desligando os sprites de ilhas selecionadas
        TurnAllIslandSelectSpriteOff();

        // Desligando a tela de dificuldade
        TurnDifficultyScreenOff();

        // Desligando a tela de seleção de personagem
        characterSelectionManager.TurnScreenOff();

        // Desligando o mapa
        mapScreen.SetActive(false);
    }
    void SetButtons() {
        foreach (var pair in dictionaryOfButtons) {  // ILHAS
            if (!dictinaryOfDescritions.TryGetValue(pair.Key, out var description)) continue;

            Button button = pair.Value;
            var localDescription = description;

            button.onClick.AddListener(() => InslandButtonFunc(button, localDescription));
        }

        closeMapButton.onClick.AddListener(() => {
            TurnMapOff();
        });



        testIslandButton.onClick.AddListener(() => bossDifficultyManager.TurnBossDifficultyScreenOn(TestIslandDescription));
    }

    void InslandButtonFunc(Button button, BossDescription description) {
        if (button != _selectedIslandButton) {
            TurnDifficultyScreenOn(description);
        }
        else {
            TurnDifficultyScreenOff();
        }
    }

    void TurnDifficultyScreenOn(BossDescription description) {

        bossDifficultyManager.TurnBossDifficultyScreenOn(description);

        TurnIslandSelectSpriteOn(description.Boss);

        closeMapButton.gameObject.SetActive(false);
    }

    public void TurnDifficultyScreenOff(bool turnDifficultySreenOff = true) {

        if (turnDifficultySreenOff) bossDifficultyManager.TurnBossDifficultyScreenOff();

        TurnAllIslandSelectSpriteOff();

        closeMapButton.gameObject.SetActive(true);
    }
    #endregion

    #region Map Initialization
    public void InitializeMap(PlayerInputHandlerManager handler = null) {
        _handler = handler;

        WhiteBoard board = WhiteBoard.Instance;

        foreach (var boss in board.ReturnListOfUnlockedPhasesByBoss().Keys) { // FOGS
            if (dictionaryOfFogs.ContainsKey(boss)) {
                dictionaryOfFogs[boss].SetActive(false);
            }
        }

        foreach (var phase in dictionaryOfButtons.Keys) { // ILHAS

            Button button = dictionaryOfButtons[phase].GetComponent<Button>();
            button.interactable = board.ReturnListOfUnlockedPhasesByBoss().ContainsKey(phase);
        }

        mapScreen.SetActive(true);
    }

    #endregion

    #region Change Island Sprite Methods
    void TurnIslandSelectSpriteOn(Bosses bossRelatedToTheIsland) {
        foreach (var pair in dictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Key == bossRelatedToTheIsland) {
                image.sprite = dictionaryOfSelectedSprites[pair.Key];
                _selectedIslandButton = pair.Value;
            }
            else image.sprite = dictionaryOfUnselectedSprites[pair.Key];
        }
    }
    public void TurnAllIslandSelectSpriteOff() {
        foreach (var pair in dictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            image.sprite = dictionaryOfUnselectedSprites[pair.Key];
        }

        _selectedIslandButton = null;
    }
    public void TurnIslandHooverSpriteOn(Button bossRelatedToTheIsland) {

        if (!dictionaryOfButtons.ContainsValue(bossRelatedToTheIsland) || _selectedIslandButton == bossRelatedToTheIsland || !bossRelatedToTheIsland.interactable) return;

        foreach (var pair in dictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Value == bossRelatedToTheIsland) image.sprite = dictionaryOfHoverSprites[pair.Key];
        }
    }
    public void TurnIslandSelectSpriteOff(Button bossRelatedToTheIsland) {

        if (!dictionaryOfButtons.ContainsValue(bossRelatedToTheIsland) || _selectedIslandButton == bossRelatedToTheIsland || !bossRelatedToTheIsland.interactable) return;

        foreach (var pair in dictionaryOfButtons) {
            pair.Value.TryGetComponent(out Image image);
            if (pair.Value == bossRelatedToTheIsland) image.sprite = dictionaryOfUnselectedSprites[pair.Key];
        }
    }

    #endregion

    #region Button Methods
    public void EnterExitButton(Image exitImage) {
        exitImage.sprite = enterExitButtonSprite;
    }
    public void ExitExitButton(Image exitImage) {
        exitImage.sprite = exitExitButtonSprite;
    }
    #endregion
}
