using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField, Foldout("Components")] GameObject firstButtonSelected;

    [SerializeField, Foldout("Screens")] GameObject configurationScreen;
    [SerializeField, Foldout("Screens")] ConfigurationScreen firstScreen;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " Gameobject")] SerializedDictionary<ConfigurationScreen, ConfigScreen> screens;
    [SerializeField, Foldout("Screens"), SerializedDictionary("Type of Screen", " String")] SerializedDictionary<ConfigurationScreen, LocalizedString> screensTitles;

    [SerializeField, Foldout("Buttons")] Button closeConfigurationScreenButton;
    [SerializeField, Foldout("Buttons"), SerializedDictionary("Type of Screen", " Button")] SerializedDictionary<ConfigurationScreen, Button> screenButtons;

    [SerializeField, Foldout("Sprites"), SerializedDictionary("Type of Screen", " Sprite")] SerializedDictionary<ConfigurationScreen, Sprite> unselectedSprites;
    [SerializeField, Foldout("Sprites"), SerializedDictionary("Type of Screen", " Sprite")] SerializedDictionary<ConfigurationScreen, Sprite> selectedSprites;
    [SerializeField, Foldout("Sprites")] Sprite unselectedBackground;
    [SerializeField, Foldout("Sprites")] Sprite selectedBackground;

    [SerializeField, Foldout("Input")] InputActionReference RBButton;
    [SerializeField, Foldout("Input")] InputActionReference LBButton;
    [SerializeField, Foldout("Input")] InputActionReference cancelButton;

    public event Action OnConfigurationScreenClose;


    ConfigurationScreen _currentScreen;
    List<ConfigurationScreen> _screensOrder;

    #region Awake and Setup

    private void Awake() {
        SetButtonsFunctions();
        configurationScreen.SetActive(false);

        _screensOrder = screens.Keys.ToList();
    }
    private void OnDestroy() {
        RBButton.action.performed -= RBMethod;
        LBButton.action.performed -= LBMethod;
        cancelButton.action.performed -= CancelMethod;
    }

    void SetButtonsFunctions() {
        closeConfigurationScreenButton.onClick.AddListener(() => {
            CloseConfigurationScreen();
        });

        foreach (var button in screenButtons) {
            var screenType = button.Key;
            button.Value.onClick.AddListener(() => TurnScreenOn(screenType));
        }

        RBButton.action.performed += RBMethod;
        LBButton.action.performed += LBMethod;
        cancelButton.action.performed += CancelMethod;
    }


    public void CloseConfigurationScreen() {
        if (!configurationScreen.activeInHierarchy) return;

        Debug.Log("Close Config");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonSelected);


        configurationScreen.SetActive(false);

        OnConfigurationScreenClose?.Invoke();

    }
    #endregion

    #region Initialization

    public void InitializeConfigurationScreen() {
        configurationScreen.SetActive(true);
        TurnScreenOn(firstScreen);
    }

    #endregion

    #region GenericScreens

    void TurnScreenOn(ConfigurationScreen screenType) {
        foreach (var screen in screens) {
            ConfigurationScreen current = screen.Key;
            screen.Value.HandleConfigurationScreen(current == screenType);
        }

        foreach (var button in screenButtons) {
            ConfigurationScreen current = button.Key;
            button.Value.image.sprite = current == screenType ? selectedSprites[current] : unselectedSprites[current];
        }

        screenTitle.StringReference = screensTitles[screenType];
        _currentScreen = screenType;
    }

    #region Unity Events Methods
    public void SetHooverButtonBackground(Transform hooverPosition) {
        hooverBackground.SetActive(true);
        hooverBackground.transform.position = hooverPosition.position;
    }

    public void DisableHooverButtonBackground() {
        hooverBackground.SetActive(false);
    }
    public void SetSelectedBackground(Image buttonBackground) => buttonBackground.sprite = selectedBackground;
    public void SetUnselectedBackgroung(Image ButtonBackGround) => ButtonBackGround.sprite = unselectedBackground;
    #endregion

    #region Input Methods
    public void RBMethod(InputAction.CallbackContext ctx) {

        if (!ctx.performed || !configurationScreen.activeInHierarchy) return;

        int currentIndex = _screensOrder.IndexOf(_currentScreen);
        int nextIndex = (currentIndex + 1) % _screensOrder.Count;
        TurnScreenOn(_screensOrder[nextIndex]);
    }
    public void LBMethod(InputAction.CallbackContext ctx) {

        if (!ctx.performed || !configurationScreen.activeInHierarchy) return;

        int currentIndex = _screensOrder.IndexOf(_currentScreen);
        int nextIndex = (currentIndex - 1 + _screensOrder.Count) % _screensOrder.Count;
        TurnScreenOn(_screensOrder[nextIndex]);
    }

    public void CancelMethod(InputAction.CallbackContext ctx) {

        if (!ctx.performed) return;

        CloseConfigurationScreen();
    }

    #endregion

    #endregion

}
