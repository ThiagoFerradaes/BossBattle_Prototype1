using System;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Threading;

public class LoadingScreenManager : MonoBehaviour {
    // Components
    [Foldout("Tip"), SerializeField] TextMeshProUGUI tipTitle;
    [Foldout("Tip"), SerializeField] TextMeshProUGUI tipText;
    [Foldout("Tip"), SerializeField] float tipDuration;
    [Foldout("Tip"), SerializeField] float tipChangingDuration;
    [Foldout("Tip"), SerializeField] GameObject tipObject;

    [Foldout("Save"), SerializeField] Image bossSavingIcon;
    [Foldout("Save"), SerializeField] GameObject savingIcon;
    [Foldout("Save"), SerializeField] float saveIconFadeTime;
    [Foldout("Save"), SerializeField] float maxSaveIconAlpha;
    [Foldout("Save"), SerializeField] float minSaveIconAlpha;

    [Foldout("Loading"), SerializeField] Image loadingBar;
    [Foldout("Loading"), SerializeField] float loadingScreenMinTime;


    public static LoadingScreenManager Instance;
    public static LoadingScreenSO CurrentLoadingScreenInfo = null;
    Coroutine savingFadeCoroutine, tipCoroutine;

    AsyncOperation loadingOperation;
    bool isLoadingComplete = false;
    bool canLoad = false;
    float loadingScreenTimer;

    #region Start Region
    private void Start() {
        StartLoad();
    }

    void StartLoad() {

        Application.runInBackground = true;

        Time.timeScale = 1.0f;

        isLoadingComplete = false;
        canLoad = false;
        loadingScreenTimer = 0.0f;

        Load();
    }
    void Load() {
        //ChooseRandomBackground(CurrentLoadingScreenInfo);
        savingFadeCoroutine ??= StartCoroutine(SavingIconFade());
        tipCoroutine ??= StartCoroutine(HandleTipChanging());

        loadingOperation = SceneManager.LoadSceneAsync(CurrentLoadingScreenInfo.SceneIndex);
        loadingOperation.allowSceneActivation = false;

        canLoad = true;

    }


    #region Coroutines
    IEnumerator SavingIconFade()
    {
        bossSavingIcon.sprite = CurrentLoadingScreenInfo.SavingIcon;

        CanvasGroup canvasG = savingIcon.GetComponent<CanvasGroup>();

        while (true)
        {

            yield return canvasG.DOFade(minSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

            yield return canvasG.DOFade(maxSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

        }
    }

    IEnumerator HandleTipChanging()
    {

        List<Tip> list = new(CurrentLoadingScreenInfo.ListOfTips);

        CanvasGroup canvasG = tipObject.GetComponent<CanvasGroup>();

        int rng = Random.Range(0, list.Count);

        tipTitle.text = $"Tip #{list[rng].TipIndex}";

        tipText.text = list[rng].TipDescription;

        list.RemoveAt(rng);

        while (true)
        {
            yield return new WaitForSecondsRealtime(tipDuration);

            yield return canvasG.DOFade(minSaveIconAlpha, tipChangingDuration).SetUpdate(true).WaitForCompletion();

            rng = Random.Range(0, list.Count);

            tipTitle.text = $"Tip #{list[rng].TipIndex} ";

            tipText.text = list[rng].TipDescription;

            list.RemoveAt(rng);

            yield return canvasG.DOFade(maxSaveIconAlpha, tipChangingDuration).SetUpdate(true).WaitForCompletion();

        }
    }
    #endregion

    #endregion

    #region Update Region
    private void Update() {
        if (!canLoad)  return;


        if (!isLoadingComplete) {
            loadingScreenTimer += Time.deltaTime;
            float timeProgress = Mathf.Clamp01(loadingScreenTimer / loadingScreenMinTime);
            float sceneProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);

            float progress = Mathf.Min(timeProgress, sceneProgress);
            loadingBar.fillAmount = progress;

            if (loadingScreenTimer >= loadingScreenMinTime && loadingOperation.progress >= 0.9f)
                EndLoad();
        }
    }
    void EndLoad()
    {
        if (savingFadeCoroutine != null)
        {
            StopCoroutine(savingFadeCoroutine);
            savingFadeCoroutine = null;
        }
        if (tipCoroutine != null)
        {
            StopCoroutine(tipCoroutine);
            tipCoroutine = null;
        }

        DOTween.KillAll();

        Application.runInBackground = true;

        isLoadingComplete = true;
        loadingOperation.allowSceneActivation = true;
        loadingOperation = null;
    }

    #endregion

    #region Canceled
    //[Foldout("BackGround"), SerializeField] Image backGroundImage;

    //void ChooseRandomBackground(LoadingScreenSO loadingScriptable) {
    //    List<Sprite> list = new(loadingScriptable.ListOfBackgrounds);

    //    int rng = Random.Range(0, list.Count);

    //    Sprite newSprite = list[rng];

    //    backGroundImage.sprite = newSprite;

    //    bossSavingIcon.sprite = loadingScriptable.SavingIcon;
    //}

    #endregion
}

