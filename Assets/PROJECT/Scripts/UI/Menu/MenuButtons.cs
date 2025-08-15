using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;

    [Header ("Temporario")]
    [SerializeField] GameObject Map;

    private void Start() {
        //startButton.onClick.AddListener(() => LoadingScreenManager.Instance.ReturnToTavern());
        startButton.onClick.AddListener(OpenMap);
        exitButton.onClick.AddListener(ExitGame);
    }

    void OpenMap() {
        Map.SetActive(true);
    }

    void ExitGame() {
        Application.Quit();
    }
}
