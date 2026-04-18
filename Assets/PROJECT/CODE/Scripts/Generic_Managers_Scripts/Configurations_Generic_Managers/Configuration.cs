using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public enum ConfigurationScreen {
    Gameplay,
    Graphics,
    Audio,
    Language,
    Tutorial
}
public class Configuration : MonoBehaviour {

    [SerializeField, Foldout("Components")] LocalizeStringEvent screenTitle;
    [SerializeField, Foldout("Components")] GameObject hooverBackground;

    [SerializeField, Foldout("Screens")] GameObject configurationScreen;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " Gameobject")] SerializedDictionary<ConfigurationScreen, ConfigScreen> screens;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " String")] SerializedDictionary<ConfigurationScreen, LocalizedString> screensTitles;

    [SerializeField, Foldout("Buttons")] Button closeConfigurationScreenButton;
    [SerializeField, Foldout("Buttons"), SerializedDictionary("Type of Screen", " Button")] SerializedDictionary<ConfigurationScreen, Button> screenButtons;

    [SerializeField, Foldout("Sprites"), SerializedDictionary("Type of Screen", " Sprite")] SerializedDictionary<ConfigurationScreen, Sprite> unselectedSprites;
    [SerializeField, Foldout("Sprites"), SerializedDictionary("Type of Screen", " Sprite")] SerializedDictionary<ConfigurationScreen, Sprite> selectedSprites;



    #region Awake and Setup

    private void Awake() {
        SetButtonsFunctions();
        configurationScreen.SetActive(false);
    }

    void SetButtonsFunctions() {
        closeConfigurationScreenButton.onClick.AddListener(() => {
            CloseConfigurationScreen();
        });

        foreach (var button in screenButtons) {
            var screenType = button.Key;
            button.Value.onClick.AddListener(() => TurnScreenOn(screenType));
        }
    }


    void CloseConfigurationScreen() {
        configurationScreen.SetActive(false);
    }
    #endregion

    #region Initialization

    public void InitializeConfigurationScreen() {
        configurationScreen.SetActive(true);
        TurnScreenOn(ConfigurationScreen.Gameplay);
    }

    #endregion

    #region GenericScreens

    void TurnScreenOn(ConfigurationScreen screenType) {
        foreach (var screen in screens) {
            screen.Value.HandleConfigurationScreen(screen.Key == screenType);
        }

        foreach(var button in screenButtons) {
            button.Value.image.sprite = button.Key == screenType ? selectedSprites[button.Key] : unselectedSprites[button.Key];
        }

        screenTitle.StringReference = screensTitles[screenType];
    }

    public void SetHooverButtonBackground(Transform hooverPosition) {
        hooverBackground.SetActive(true);
        hooverBackground.transform.position = hooverPosition.position;
    }

    public void DisableHooverButtonBackground() {
        hooverBackground.SetActive(false);
    }

    #endregion

}
