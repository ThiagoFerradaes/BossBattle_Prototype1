using System;
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

        menuButton.onClick.AddListener(() => {
            LoadingScreenManager.CurrentLoadingScreenInfo = menuScreenInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });

        continueButton.onClick.AddListener(() => TurnScreenOff());

        quitButton.onClick.AddListener(() => Application.Quit());

        configButton.onClick.AddListener(() => configScreen.InitializeConfigurationScreen());

        cancelAction.action.performed += CancelButton;
    }

    public void TurnScreenOn() {
        Time.timeScale = 0;

        pauseScreen.SetActive(true);

        HandleControlSystem();
    }

    void HandleControlSystem() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        InputDeviceManager.Instance.ChangeCurrentSelectedButton(continueButton.gameObject);
    }

    void CancelButton(InputAction.CallbackContext context) {
        if (!context.performed || !pauseScreen.activeInHierarchy) return;

        TurnScreenOff();
    }
    public void TurnScreenOff() {
        Time.timeScale = 1;

        pauseScreen.SetActive(false);

        TurnButtonBackgroundOff();

        configScreen.CloseConfigurationScreen();
    }

    public void TurnButtonBackgroundOn(Transform target) {
        backgroundForButtons.transform.position = target.position;
        backgroundForButtons.SetActive(true);
    }

    public void TurnButtonBackgroundOff() {
        backgroundForButtons.SetActive(false);
    }
}
