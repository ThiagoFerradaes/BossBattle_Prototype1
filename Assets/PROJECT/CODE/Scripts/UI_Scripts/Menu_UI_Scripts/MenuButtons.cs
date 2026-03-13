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
    [SerializeField] Button exitConfigButton;
    [SerializeField] Button yesExitPopUp;
    [SerializeField] Button noExitPopUp;
    [SerializeField] Button maskExitPopUp;
    [SerializeField] Image hoverButtonBackGround;
    [SerializeField] GameObject exitPopUp;

    [Header ("Temporario")]
    [SerializeField] GameObject map;
    [SerializeField] LoadingScreenSO tavernLoadingScreenInfo;

    [Header("Canvas")]
    [SerializeField] GameObject configCanvas;

    private void Start() {
        tavernButton.onClick.AddListener(() => {
            LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingScreenInfo;
            SceneManager.LoadScene(1);
        });
        startButton.onClick.AddListener(OpenMap);
        configButton.onClick.AddListener(() => HandleConfigUI(true));
        exitConfigButton.onClick.AddListener(() => HandleConfigUI(false));

        // Exit game Buttons
        exitButton.onClick.AddListener(() => HandleExitPopUp(true));
        noExitPopUp.onClick.AddListener(() => HandleExitPopUp(false));
        yesExitPopUp.onClick.AddListener(ExitGame);
        maskExitPopUp.onClick.AddListener(() => HandleExitPopUp(false));
    }

    void OpenMap() {
        map.SetActive(true);
    }

    void HandleConfigUI(bool open) {
        configCanvas.SetActive(open);
    }

    public void HandleButtonBackGroundOn(Button button) {
        hoverButtonBackGround.transform.position = button.transform.position;
        hoverButtonBackGround.gameObject.SetActive(true);
    }

    public void HandleButtonBackGroundOff() {
        hoverButtonBackGround.gameObject.SetActive(false);
    }

    void HandleExitPopUp(bool on) {
        exitPopUp.gameObject.SetActive(on);
    }
    void ExitGame() {
        Application.Quit();
    }
}
