using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour {
    public static PauseScreen Instance;

    [SerializeField] Button continueButton;
    [SerializeField] Button configButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button quitButton;

    [SerializeField] GameObject backgroundForButtons;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] LoadingScreenSO menuScreenInfo;
    [SerializeField] Configuration configScreen;
    [SerializeField] InputActionReference cancelAction;

    [Header("Animation")]
    [SerializeField] EnterExitAnimationManager enterAndExitAnimator;

    public event Action OnDespause;

    // Coroutines
    Coroutine _endAnimationCoroutine;

    // AsyncOperation
    AsyncOperation _loadSceneAsyncOperation;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        configScreen.CloseConfigurationScreen();
        configScreen.OnConfigurationScreenClose += HandleControlSystem;
        TurnScreenOff();

    }
    private void Start() {
        SetButton();
    }

    private void OnDestroy() {
        configScreen.OnConfigurationScreenClose -= HandleControlSystem;
    }
    void SetButton() {

        menuButton.onClick.AddListener(MenuButton);

        continueButton.onClick.AddListener(() => TurnScreenOff());

        quitButton.onClick.AddListener(() => Application.Quit());

        configButton.onClick.AddListener(() => configScreen.InitializeConfigurationScreen());


    }

    void MenuButton() {
        LoadingScreenManager.CurrentLoadingScreenInfo = menuScreenInfo;
        _loadSceneAsyncOperation = SceneManager.LoadSceneAsync(1);
        _loadSceneAsyncOperation.allowSceneActivation = false;

        _endAnimationCoroutine ??= StartCoroutine(EndAnimationCoroutine());
    }

    IEnumerator EndAnimationCoroutine() {

        yield return enterAndExitAnimator.ReturnExitAnimationCoroutine(true);

        _endAnimationCoroutine = null;
        
        _loadSceneAsyncOperation.allowSceneActivation = true;
        Time.timeScale = 1;
    }
    public void Pause() {
        if (Time.timeScale == 1 && !pauseScreen.activeInHierarchy) {
            TurnScreenOn();
        }
        else TurnScreenOff();
    }
    void TurnScreenOn() {
        Time.timeScale = 0;

        pauseScreen.SetActive(true);

        //HandleControlSystem();
    }

    void HandleControlSystem() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        InputDeviceManager.Instance.ChangeCurrentSelectedButton(continueButton.gameObject);
    }

    void CancelButton(InputAction.CallbackContext context) {
        if (!context.performed || !pauseScreen.activeInHierarchy) return;

        Debug.Log("Cancel button");

        TurnScreenOff();
    }
    void TurnScreenOff() {
        Time.timeScale = 1;

        pauseScreen.SetActive(false);

        TurnButtonBackgroundOff();

        configScreen.CloseConfigurationScreen();

        OnDespause?.Invoke();
    }

    public void TurnButtonBackgroundOn(Transform target) {
        backgroundForButtons.transform.position = target.position;
        backgroundForButtons.SetActive(true);
    }

    public void TurnButtonBackgroundOff() {
        backgroundForButtons.SetActive(false);
    }
}
