using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button configButton;
    [SerializeField] Button exitConfigButton;

    [Header ("Temporario")]
    [SerializeField] GameObject Map;

    [Header("Canvas")]
    [SerializeField] GameObject configCanvas;

    private void Start() {
        //startButton.onClick.AddListener(() => LoadingScreenManager.Instance.ReturnToTavern());
        startButton.onClick.AddListener(OpenMap);
        exitButton.onClick.AddListener(ExitGame);
        configButton.onClick.AddListener(() => HandleConfigUI(true));
        exitConfigButton.onClick.AddListener(() => HandleConfigUI(false));
    }

    void OpenMap() {
        Map.SetActive(true);
    }

    void HandleConfigUI(bool open) {
        configCanvas.SetActive(open);
    }

    void ExitGame() {
        Application.Quit();
    }
}
