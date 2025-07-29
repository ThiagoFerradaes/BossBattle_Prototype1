using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;

    private void Start() {
        startButton.onClick.AddListener(ChangeScene);
        exitButton.onClick.AddListener(ExitGame);
    }

    void ChangeScene() {
        SceneManager.LoadScene(1);
    }

    void ExitGame() {
        Application.Quit();
    }
}
