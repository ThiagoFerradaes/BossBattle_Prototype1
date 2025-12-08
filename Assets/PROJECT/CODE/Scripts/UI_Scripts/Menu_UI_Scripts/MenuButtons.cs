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
    [SerializeField] Button tavernButton;

    [Header ("Temporario")]
    [SerializeField] GameObject Map;

    [Header("Canvas")]
    [SerializeField] GameObject configCanvas;

    private void Start() {
        tavernButton.onClick.AddListener(() => LoadingScreenManager.Instance.ReturnToTavern(true, 5));
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
