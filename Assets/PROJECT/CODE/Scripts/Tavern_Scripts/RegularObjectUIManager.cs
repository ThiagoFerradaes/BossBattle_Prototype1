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

    Coroutine screenDurationCoroutine;
    WaitForSeconds screenDurationWaitForSeconds;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
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
        screen.SetActive(true);
        alphaGroup.alpha = 1;
        yield return screenDurationWaitForSeconds;

        float timer = 0f;

        while (timer < fadeOutDuration) {
            timer += Time.deltaTime;
            alphaGroup.alpha = 1 - (timer / fadeOutDuration);
            yield return null;
        }

        screen.SetActive(false);
        screenDurationCoroutine = null;
    }
}
