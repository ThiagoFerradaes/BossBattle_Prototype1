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
        startButton.onClick.AddListener(ChangeScene);
        exitButton.onClick.AddListener(ExitGame);
    }

    void ChangeScene() {
        Map.SetActive(true);
    }

    void ExitGame() {
        Application.Quit();
    }
}
