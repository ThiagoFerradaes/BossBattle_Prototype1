using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenManager : MonoBehaviour
{
    public static VictoryScreenManager Instance;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] Button menuButton;
    [SerializeField] Button tavernButton;
    [SerializeField] LoadingScreenSO menuLoadingInfo;
    [SerializeField] LoadingScreenSO tavernLoadingInfo;
    [SerializeField] AK.Wwise.Event victoryMusic;
    [SerializeField] EnterExitAnimationManager enterExitAnimationManager;

    // Coroutines 
    Coroutine _exitAnimationCoroutine;

    // AsyncOperation
    AsyncOperation _loadingOperation;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TurnScreenOff();
        SetButton();
    }
    void SetButton()
    {
        menuButton.onClick.AddListener(() => Exit(MenuButton));
        tavernButton.onClick.AddListener(() => Exit(TavernButton));
    }

    public void InitializeVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0;

        AkUnitySoundEngine.StopAll();
        victoryMusic.Post(gameObject);
    }

    void MenuButton() {
        LoadingScreenManager.CurrentLoadingScreenInfo = menuLoadingInfo;
        _loadingOperation = SceneManager.LoadSceneAsync(1);

        _loadingOperation.allowSceneActivation = false;
    }

    void TavernButton() {
        LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingInfo;
        _loadingOperation = SceneManager.LoadSceneAsync(1);
        _loadingOperation.allowSceneActivation = false;
    }

    void TurnScreenOff()
    {
        victoryScreen.SetActive(false);
    }

    void Exit(Action postYieldAction) {
        _exitAnimationCoroutine ??= StartCoroutine(ExitRoutine(postYieldAction));
    }

    IEnumerator ExitRoutine(Action postYieldAction) {
        postYieldAction();

        yield return enterExitAnimationManager.ReturnExitAnimationCoroutine(true);

        _exitAnimationCoroutine = null;

        _loadingOperation.allowSceneActivation = true;
        Time.timeScale = 1;
    }
}
