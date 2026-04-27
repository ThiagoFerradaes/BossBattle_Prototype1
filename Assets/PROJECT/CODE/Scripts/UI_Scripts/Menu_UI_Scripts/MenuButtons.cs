using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button configButton;
    [SerializeField] Button tavernButton;

    [Header("Components")]
    [SerializeField] Image hoverButtonBackGround;
    [SerializeField] ExitPopUpManager exitPopUp;

    [Header ("Temporario")]
    [SerializeField] MapManager map;
    [SerializeField] LoadingScreenSO tavernLoadingScreenInfo;

    [Header("Canvas")]
    [SerializeField] Configuration configCanvas;

    AsyncOperation asyncOperation;

    private void Awake() {
        tavernButton.onClick.AddListener(LoadLoadingScreen);
        startButton.onClick.AddListener(() => map.InitializeMap());
        configButton.onClick.AddListener(() => configCanvas.InitializeConfigurationScreen());

        // Exit game Buttons
        exitButton.onClick.AddListener(() => exitPopUp.HandleExitPopUp(true));
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


}
