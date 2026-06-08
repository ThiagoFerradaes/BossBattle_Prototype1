using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenManager : MonoBehaviour {
    public static DefeatScreenManager Instance;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] Button menuButton;
    [SerializeField] Button tavernButton;
    [SerializeField] Button retryButton;
    [SerializeField] LoadingScreenSO menuLoadingInfo;
    [SerializeField] LoadingScreenSO tavernLoadingInfo;
    [SerializeField] AK.Wwise.Event defeatMusic;

    [Header("Animation")]
    [SerializeField] EnterExitAnimationManager enterExitAnimationManager;

    // Coroutines
    Coroutine _exitAnimationCoroutine;

    // AsyncOperation
    AsyncOperation _loadinOperation;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TurnScreenOff();
        SetButton();
    }
    void SetButton() {
        menuButton.onClick.AddListener(() => Exit(MenuButton));
        tavernButton.onClick.AddListener(() => Exit(TavernButton));
        retryButton.onClick.AddListener(() => Exit(RetryButton));
    }

    public void InitializeDefeatScreen() {
        defeatScreen.SetActive(true);
        Time.timeScale = 0;

        AkUnitySoundEngine.StopAll();
        defeatMusic.Post(gameObject);
    }

    void MenuButton() {
        LoadingScreenManager.CurrentLoadingScreenInfo = menuLoadingInfo;
        _loadinOperation = SceneManager.LoadSceneAsync(1);

        _loadinOperation.allowSceneActivation = false;
    }

    void TavernButton() {
        LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingInfo;
        _loadinOperation = SceneManager.LoadSceneAsync(1);
        _loadinOperation.allowSceneActivation = false;
    }

    void RetryButton() {
        _loadinOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        _loadinOperation.allowSceneActivation = false;
    }
    void TurnScreenOff() {
        defeatScreen.SetActive(false);
    }

    void Exit (Action postYieldAction) {
        _exitAnimationCoroutine ??= StartCoroutine(ExitRoutine(postYieldAction));
    }

    IEnumerator ExitRoutine(Action postYieldAction) {
        postYieldAction();

        yield return enterExitAnimationManager.ReturnExitAnimationCoroutine(true);

        _exitAnimationCoroutine = null;

        _loadinOperation.allowSceneActivation = true;
        Time.timeScale = 1;
    }
}
