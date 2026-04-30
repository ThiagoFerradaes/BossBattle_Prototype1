using System;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Localization.Components;

public class LoadingScreenManager : MonoBehaviour {
    // Components
    [Foldout("Tip"), SerializeField] TextMeshProUGUI tipTitle;
    [Foldout("Tip"), SerializeField] TextMeshProUGUI tipText;
    [Foldout("Tip"), SerializeField] float tipDuration;
    [Foldout("Tip"), SerializeField] float tipChangingDuration;
    [Foldout("Tip"), SerializeField] GameObject tipObject;
    [Foldout("Tip"), SerializeField, Range(0,1)] float maxTipAlpha;
    [Foldout("Tip"), SerializeField, Range(0,1)] float minTipAlpha;

    [Foldout("Save"), SerializeField] LocalizeSpriteEvent bossSavingIcon;
    [Foldout("Save"), SerializeField] GameObject savingIcon;
    [Foldout("Save"), SerializeField] float saveIconFadeTime;
    [Foldout("Save"), SerializeField, Range(0,1)] float maxSaveIconAlpha;
    [Foldout("Save"), SerializeField, Range(0,1)] float minSaveIconAlpha;

    [Foldout("Loading"), SerializeField] Image loadingBar;
    [Foldout("Loading"), SerializeField] float loadingScreenMinTime;


    public static LoadingScreenManager Instance;
    public static LoadingScreenSO CurrentLoadingScreenInfo = null;
    Coroutine savingFadeCoroutine, loadCoroutine;

    AsyncOperation loadingOperation;
    bool isLoadingComplete = false;
    bool canLoad = false;
    float loadingScreenTimer;

    WaitForSecondsRealtime _realSecondsToWait = new(0.1f);
    WaitForEndOfFrame _waitForFrame = new();

    #region Load Flow
    private void Start() {
        StartLoad();
    }

    void StartLoad() {

        //AkUnitySoundEngine.StopAll();

        Application.runInBackground = true;

        Time.timeScale = 1.0f;

        isLoadingComplete = false;
        canLoad = false;
        loadingScreenTimer = 0.0f;

        loadCoroutine ??= StartCoroutine(Load());
    }
    IEnumerator Load() {

        savingFadeCoroutine ??= StartCoroutine(SavingIconFade());

        HandleTip();

        yield return _realSecondsToWait;
        yield return _waitForFrame;

        loadingOperation = SceneManager.LoadSceneAsync(CurrentLoadingScreenInfo.SceneIndex);
        loadingOperation.allowSceneActivation = false;

        canLoad = true;

        loadCoroutine = null;

    }
    private void Update() {
        if (!canLoad) return;


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
    void EndLoad() {
        if (savingFadeCoroutine != null) {
            StopCoroutine(savingFadeCoroutine);
            savingFadeCoroutine = null;
        }

        DOTween.KillAll();

        Application.runInBackground = true;

        isLoadingComplete = true;
        loadingOperation.allowSceneActivation = true;
        loadingOperation = null;
    }

    #region UI Elements
    IEnumerator SavingIconFade()
    {
        bossSavingIcon.AssetReference = CurrentLoadingScreenInfo.SavingIcon;

        CanvasGroup canvasG = savingIcon.GetComponent<CanvasGroup>();

        while (true)
        {

            yield return canvasG.DOFade(minSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

            yield return canvasG.DOFade(maxSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

        }
    }

    void HandleTip()
    {
        List<Tip> list = new(CurrentLoadingScreenInfo.ListOfTips);

        int rng = Random.Range(0, list.Count);

        tipText.text = list[rng].TipDescription.GetLocalizedString();

    }
    #endregion

    #endregion


}

