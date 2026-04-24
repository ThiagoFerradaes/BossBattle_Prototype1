using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button configButton;
    [SerializeField] Button tavernButton;
    [SerializeField] Button yesExitPopUp;
    [SerializeField] Button noExitPopUp;
    [SerializeField] Button maskExitPopUp;
    [SerializeField] Image hoverButtonBackGround;
    [SerializeField] GameObject exitPopUp;

    [Header ("Temporario")]
    [SerializeField] MapManager map;
    [SerializeField] LoadingScreenSO tavernLoadingScreenInfo;

    [Header("Canvas")]
    [SerializeField] Configuration configCanvas;

    AsyncOperation asyncOperation;

    private void Start() {
        tavernButton.onClick.AddListener(LoadLoadingScreen);
        startButton.onClick.AddListener(() => map.InitializeMap());
        configButton.onClick.AddListener(() => configCanvas.InitializeConfigurationScreen());

        // Exit game Buttons
        exitButton.onClick.AddListener(() => HandleExitPopUp(true));
        noExitPopUp.onClick.AddListener(() => HandleExitPopUp(false));
        yesExitPopUp.onClick.AddListener(ExitGame);
        maskExitPopUp.onClick.AddListener(() => HandleExitPopUp(false));
    }
    void LoadLoadingScreen() {
        LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingScreenInfo;
        asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
    public void HandleButtonBackGroundOn(Button button) {
        hoverButtonBackGround.transform.position = button.transform.position;
        hoverButtonBackGround.gameObject.SetActive(true);
    }

    public void HandleButtonBackGroundOff() {
        hoverButtonBackGround.gameObject.SetActive(false);
    }

    void HandleExitPopUp(bool on) {
        exitPopUp.SetActive(on);
    }
    void ExitGame() {
        Application.Quit();
    }
}
