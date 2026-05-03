using System.Collections;
using TMPro;
using UnityEngine;

public class RegularObjectUIManager : MonoBehaviour {

    // Singleton
    public static RegularObjectUIManager Instance;

    [Header("Components")]
    [SerializeField] GameObject interactionScreen;
    [SerializeField] GameObject interactScreen;
    [SerializeField] TextMeshProUGUI lineText;
    [SerializeField] CanvasGroup interactionAlphaGroup;
    [SerializeField] CanvasGroup interactAlphaGroup;

    [Header("Atributes")]
    [SerializeField] float screenDuration = 2f;
    [SerializeField] float fadeOutDuration = 1f;
    [SerializeField] float fadeInDuration = 0.2f;

    Coroutine screenDurationCoroutine;
    WaitForSeconds screenDurationWaitForSeconds;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
        TurnScreenOff();
        TurnInteractScreenOff();
        SetVariables();
    }

    void SetVariables() {
        screenDurationWaitForSeconds = new WaitForSeconds(screenDuration);
    }
    public void InitializeInteractionScreen(string text) {
        lineText.text = text;

        screenDurationCoroutine ??= StartCoroutine(ScreenDuration());
    }

    IEnumerator ScreenDuration() {

        interactionAlphaGroup.alpha = 0;

        interactionScreen.SetActive(true);

        float timer = 0f;

        while (timer < fadeInDuration) {
            timer += Time.deltaTime;
            interactionAlphaGroup.alpha = timer / fadeInDuration;
            yield return null;
        }

        interactionAlphaGroup.alpha = 1;

        timer = 0f;

        yield return screenDurationWaitForSeconds;

        while (timer < fadeOutDuration) {
            timer += Time.deltaTime;
            interactionAlphaGroup.alpha = 1 - (timer / fadeOutDuration);
            yield return null;
        }

        TurnScreenOff();
        screenDurationCoroutine = null;
    }

    void TurnScreenOff() {
        interactionScreen.SetActive(false);
    }

    public void InitializeInteractScreen() {

        interactScreen.SetActive(true);
    }

    public void TurnInteractScreenOff() {
        interactScreen.SetActive(false);
    }

    //IEnumerator FadeIn(GameObject obj, CanvasGroup alpha) {
    //    alpha.alpha = 0;

    //    obj.SetActive(true);

    //    float timer = 0f;

    //    while (timer < fadeInDuration) {
    //        timer += Time.deltaTime;
    //        alpha.alpha = timer / fadeInDuration;
    //        yield return null;
    //    }

    //    alpha.alpha = 1;
    //}
}
