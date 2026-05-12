using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenManager : MonoBehaviour {
    public static DefeatScreenManager Instance;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] Button menuButton;
    [SerializeField] Button tavernButton;
    [SerializeField] Button retryButton;
    [SerializeField] LoadingScreenSO menuLoadingInfo;
    [SerializeField] LoadingScreenSO tavernLoadingInfo;
    [SerializeField] AK.Wwise.Event defeatMusic;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TurnScreenOff();
        SetButton();
    }
    void SetButton() {
        menuButton.onClick.AddListener(() => {
            LoadingScreenManager.CurrentLoadingScreenInfo = menuLoadingInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });
        tavernButton.onClick.AddListener(() => {
            LoadingScreenManager.CurrentLoadingScreenInfo = tavernLoadingInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });
        retryButton.onClick.AddListener(RetryButton);
    }

    public void InitializeDefeatScreen() {
        defeatScreen.SetActive(true);
        Time.timeScale = 0;

        AkUnitySoundEngine.StopAll();
        defeatMusic.Post(gameObject);
    }

    void RetryButton() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    void TurnScreenOff() {
        defeatScreen.SetActive(false);
    }
}
