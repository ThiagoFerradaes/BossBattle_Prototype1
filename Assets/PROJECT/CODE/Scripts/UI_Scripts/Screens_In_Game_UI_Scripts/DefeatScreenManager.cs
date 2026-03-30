using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenManager : MonoBehaviour
{
    public static DefeatScreenManager Instance;
    [SerializeField] GameObject defeatScreen;
    [SerializeField] Button menuButton;
    [SerializeField] Button quitButton;
    [SerializeField] LoadingScreenSO menuLoadingInfo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        TurnScreenOff();
        SetButton();
    }
    void SetButton()
    {
        menuButton.onClick.AddListener(() => {
            LoadingScreenManager.CurrentLoadingScreenInfo = menuLoadingInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    public void InitializeDefeatScreen()
    {
        defeatScreen.SetActive(true);
        Time.timeScale = 0;
    }

    void TurnScreenOff()
    {
        defeatScreen.SetActive(false);
    }
}
