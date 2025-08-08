using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BackGroundManager : MonoBehaviour {
    [SerializeField] Image backGroundImage;
    [SerializeField] float timeBetweenEachImage = 5f;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] List<Sprite> _listOfImages;
    int _currentImageIndex = 0;

    private void Start() {
        StartCoroutine(ChangeImages());
    }

    IEnumerator ChangeImages() {
        while (true) {
            int rng;
            do {
                rng = Random.Range(0, _listOfImages.Count);
            } while (rng == _currentImageIndex);

            _currentImageIndex = rng;

            yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));

            backGroundImage.sprite = _listOfImages[_currentImageIndex];

            yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));

            yield return new WaitForSeconds(timeBetweenEachImage);
        }
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha, float duration) {
        float timer = 0f;
        Color color = backGroundImage.color;

        while (timer < duration) {
            float t = timer / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            backGroundImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        backGroundImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}

