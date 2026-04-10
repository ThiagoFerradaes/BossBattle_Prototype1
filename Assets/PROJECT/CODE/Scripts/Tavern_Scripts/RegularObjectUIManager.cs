using System.Collections;
using TMPro;
using UnityEngine;

public class RegularObjectUIManager : MonoBehaviour {

    // Singleton
    public static RegularObjectUIManager Instance;

    [Header("Components")]
    [SerializeField] GameObject screen;
    [SerializeField] TextMeshProUGUI lineText;
    [SerializeField] CanvasGroup alphaGroup;

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
        SetVariables();
    }

    void SetVariables() {
        screenDurationWaitForSeconds = new WaitForSeconds(screenDuration);
    }
    public void InitializeScreen(string text) {
        lineText.text = text;

        screenDurationCoroutine ??= StartCoroutine(ScreenDuration());
    }

    IEnumerator ScreenDuration() {

        alphaGroup.alpha = 0;

        screen.SetActive(true);

        float timer = 0f;

        while (timer < fadeInDuration) {
            timer += Time.deltaTime;
            alphaGroup.alpha = timer / fadeInDuration;
            yield return null;
        }

        alphaGroup.alpha = 1;

        timer = 0f;

        yield return screenDurationWaitForSeconds;

        while (timer < fadeOutDuration) {
            timer += Time.deltaTime;
            alphaGroup.alpha = 1 - (timer / fadeOutDuration);
            yield return null;
        }

        TurnScreenOff();
        screenDurationCoroutine = null;
    }

    void TurnScreenOff() {
        screen.SetActive(false);
    }
}
