using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenManager : MonoBehaviour
{
    public static VictoryScreenManager Instance;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] Button menuButton;
    [SerializeField] Button tavernButton;
    [SerializeField] LoadingScreenSO menuLoadingInfo;
    [SerializeField] LoadingScreenSO tavernLoadingInfo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TurnScreenOff();
        SetButton();
    }
    void SetButton()
    {
        menuButton.onClick.AddListener(() =>
        {
            LoadingScreenManager.CurrentLoadingScreenInfo = menuLoadingInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });
        tavernButton.onClick.AddListener(() =>
        {
            LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        }
        );
    }

    public void InitializeVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0;
    }

    void TurnScreenOff()
    {
        victoryScreen.SetActive(false);
    }
}
