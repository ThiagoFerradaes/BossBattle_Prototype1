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

    [Foldout("BackGround"), SerializeField] Image backGroundImage;

    [Foldout("Save"), SerializeField] Image bossSavingIcon;
    [Foldout("Save"), SerializeField] GameObject savingIcon;
    [Foldout("Save"), SerializeField] float saveIconFadeTime;
    [Foldout("Save"), SerializeField] float maxSaveIconAlpha;
    [Foldout("Save"), SerializeField] float minSaveIconAlpha;

    [Foldout("Loading"), SerializeField] Image loadingBar;
    [Foldout("Loading"), SerializeField] GameObject loadingScreen;

    [Foldout("Scriptable"), SerializeField] LoadingScreenSO tavernLoadingScreen;
    [Foldout("Scriptable"), SerializeField] LoadingScreenSO menuLoadingScreen;

    [Foldout("Atributes"), SerializeField] float loadingScreenTime;
    [Foldout("Atributes"), SerializeField] int tavernSceneIndex;
    [Foldout("Atributes"), SerializeField] int menuSceneIndex;


    public static LoadingScreenManager Instance;
    public static LoadingScreenSO CurrentLoadingScreenInfo = null;
    Coroutine savingFadeCoroutine, tipCoroutine;

    //public void Awake() {
    //    if (Instance == null) {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else {
    //        Destroy(gameObject);
    //    }
    //}

    //public async void ReturnToTavern(bool load = false, byte saveSlot = byte.MaxValue) {
    //    try {
    //        if (saveSlot != byte.MaxValue)
    //            await RawMaterialStatic.Instance.SetSlotSave(saveSlot);
    //        loadSceneCoroutine ??= StartCoroutine(LoadingScreen(tavernLoadingScreen, tavernSceneIndex, load));
    //    }
    //    catch (Exception e) {
    //        Debug.LogError(e);
    //    }
    //}

    //public void ReturnToMenu() {
    //    loadSceneCoroutine ??= StartCoroutine(LoadingScreen(menuLoadingScreen, menuSceneIndex));
    //}

    //public void LoadFightScene(LoadingScreenSO loadScreenInformation, int sceneIndex) {
    //    loadSceneCoroutine ??= StartCoroutine(LoadingScreen(loadScreenInformation, sceneIndex));
    //}

    AsyncOperation loadingOperation;
    public bool isLoadingComplete = false;
    public bool canLoad = false;
    public float loadingScreenTimer;

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
        ChooseRandomBackground(CurrentLoadingScreenInfo);
        savingFadeCoroutine ??= StartCoroutine(SavingIconFade());
        tipCoroutine ??= StartCoroutine(HandleTipChanging(CurrentLoadingScreenInfo));

        loadingOperation = SceneManager.LoadSceneAsync(CurrentLoadingScreenInfo.SceneIndex);
        loadingOperation.allowSceneActivation = false;

        canLoad = true;

    }

    private void Update() {
        if (!canLoad)  return;


        if (!isLoadingComplete) {
            loadingScreenTimer += Time.deltaTime;
            float timeProgress = Mathf.Clamp01(loadingScreenTimer / loadingScreenTime);
            float sceneProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);

            float progress = Mathf.Min(timeProgress, sceneProgress);
            loadingBar.fillAmount = progress;

            if (loadingScreenTimer >= loadingScreenTime && loadingOperation.progress >= 0.9f)
                EndLoad();
        }
    }
    IEnumerator SavingIconFade() {

        CanvasGroup canvasG = savingIcon.GetComponent<CanvasGroup>();

        while (true) {

            yield return canvasG.DOFade(minSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

            yield return canvasG.DOFade(maxSaveIconAlpha, saveIconFadeTime).SetUpdate(true).WaitForCompletion();

        }
    }

    IEnumerator HandleTipChanging(LoadingScreenSO loadingScriptable) {

        List<Tip> list = new(loadingScriptable.ListOfTips);

        CanvasGroup canvasG = tipObject.GetComponent<CanvasGroup>();

        int rng = Random.Range(0, list.Count);

        tipTitle.text = $"Tip #{list[rng].TipIndex}";

        tipText.text = list[rng].TipDescription;

        list.RemoveAt(rng);

        while (true) {
            yield return new WaitForSecondsRealtime(tipDuration);

            yield return canvasG.DOFade(minSaveIconAlpha, tipChangingDuration).SetUpdate(true).WaitForCompletion();

            rng = Random.Range(0, list.Count);

            tipTitle.text = $"Tip #{list[rng].TipIndex} ";

            tipText.text = list[rng].TipDescription;

            list.RemoveAt(rng);

            yield return canvasG.DOFade(maxSaveIconAlpha, tipChangingDuration).SetUpdate(true).WaitForCompletion();

        }
    }

    void ChooseRandomBackground(LoadingScreenSO loadingScriptable) {
        List<Sprite> list = new(loadingScriptable.ListOfBackgrounds);

        int rng = Random.Range(0, list.Count);

        Sprite newSprite = list[rng];

        backGroundImage.sprite = newSprite;

        bossSavingIcon.sprite = loadingScriptable.SavingIcon;
    }
    void EndLoad() {
        if (savingFadeCoroutine != null) {
            StopCoroutine(savingFadeCoroutine);
            savingFadeCoroutine = null;
        }
        if (tipCoroutine != null) {
            StopCoroutine(tipCoroutine);
            tipCoroutine = null;
        }

        DOTween.KillAll();

        Application.runInBackground = true;

        isLoadingComplete = true;
        loadingOperation.allowSceneActivation = true;
        loadingOperation = null;
    }

    //IEnumerator LoadingScreen(LoadingScreenSO loadScreenInformation, int sceneIndex, bool load = false) {

    //    ChooseRandomBackground(loadScreenInformation);

    //    savingFadeCoroutine ??= StartCoroutine(SavingIconFade());
    //    tipCoroutine ??= StartCoroutine(HandleTipChanging(loadScreenInformation));

    //    loadingScreen.SetActive(true);

    //    if (!load) {
    //        if (RawMaterialStatic.Instance is not null)
    //            yield return RawMaterialStatic.Instance.SaveInventory().AsIEnumerator();
    //    }
    //    else {
    //        yield return RawMaterialStatic.Instance.LoadInventoryByJson().AsIEnumerator();
    //    }

    //    if (RoomCanvasStatic.Instance is not null)
    //        yield return RoomCanvasStatic.Instance.SaveFurnitureByJson().AsIEnumerator();


    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    //    operation.allowSceneActivation = false;

    //    float timer = 0;

    //    while (timer < loadingScreenTime || operation.progress < 0.9f) {
    //        timer += Time.unscaledDeltaTime;

    //        float timeProgress = Mathf.Clamp01(timer / loadingScreenTime);
    //        float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f);

    //        float progress = Mathf.Min(timeProgress, sceneProgress);
    //        loadingBar.fillAmount = progress;

    //        yield return null;
    //    }

    //    operation.allowSceneActivation = true;

    //    yield return null;

    //    loadingScreen.SetActive(false);

    //    loadSceneCoroutine = null;

    //    if (savingFadeCoroutine != null) {
    //        StopCoroutine(savingFadeCoroutine);
    //        savingFadeCoroutine = null;
    //    }
    //    if (tipCoroutine != null) {
    //        StopCoroutine(tipCoroutine);
    //        tipCoroutine = null;
    //    }

    //}



}

public static class TaskExtensions {
    public static IEnumerator AsIEnumerator(this Task task) {
        while (!task.IsCompleted) {
            yield return null;
        }

        if (task.IsFaulted) {
            throw task.Exception;
        }
    }
}
