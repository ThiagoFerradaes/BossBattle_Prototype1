using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public static PauseScreen Instance;

    [SerializeField] Button continueButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button quitButton;

    [SerializeField] GameObject backgroundForButtons;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] LoadingScreenSO menuScreenInfo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        SetButton();
    }

    void SetButton()
    {

        menuButton.onClick.AddListener(() =>
        {
            LoadingScreenManager.CurrentLoadingScreenInfo = menuScreenInfo;
            SceneManager.LoadScene(1);
            Time.timeScale = 1;
        });

        continueButton.onClick.AddListener(() => TurnScreenOff());

        quitButton.onClick.AddListener(() => Application.Quit());
    }

    public void TurnScreenOn()
    {
        Time.timeScale = 0;

        pauseScreen.SetActive(true);
    }

    public void TurnScreenOff()
    {
        Time.timeScale = 1;

        pauseScreen.SetActive(false);

        TurnButtonBackgroundOff();
    }

    public void TurnButtonBackgroundOn(Transform target)
    {
        backgroundForButtons.transform.position = target.position;
        backgroundForButtons.SetActive(true);
    }

    public void TurnButtonBackgroundOff()
    {
        backgroundForButtons.SetActive(false);
    }
}
