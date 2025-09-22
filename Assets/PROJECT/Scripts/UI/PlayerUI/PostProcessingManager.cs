using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PostProcessingManager : MonoBehaviour
{
    GameObject _player;
    HealthManager _healthManager;

    [Header("Taking Damage Effect")]
    [SerializeField] float minAlpha;
    [SerializeField] float maxAlpha;
    [SerializeField] float timeToHitMaxOrMin;
    [SerializeField] int amountOfFlashes;
    [SerializeField] Image TakingDamageImage;

    // Actions
    Action<float> _onTakeDamage;

    // Coroutine
    Coroutine _onTakeDamageCoroutine;


    private void Start() {
        _player = PlayerManager.Instance.Player;

        _healthManager = _player.GetComponent<HealthManager>();

        _onTakeDamage = StartTakeDamageCoroutine;

        _healthManager.OnDamageTaken += _onTakeDamage;
    }

    private void OnDestroy() {
        _healthManager.OnDamageTaken -= _onTakeDamage;
        TakingDamageImage.DOKill();
    }
    void StartTakeDamageCoroutine(float damage) {
        _onTakeDamageCoroutine ??= StartCoroutine(TakeDamageCoroutine());
    }

    IEnumerator TakeDamageCoroutine() {

        for (int i = 0; i < amountOfFlashes; i++) {
            yield return TakingDamageImage.DOFade(maxAlpha, timeToHitMaxOrMin).WaitForCompletion();
            
            yield return TakingDamageImage.DOFade(minAlpha, timeToHitMaxOrMin).WaitForCompletion();

        }

        Color alpha = TakingDamageImage.color;
        alpha.a = 0f;
        TakingDamageImage.color = alpha;

        _onTakeDamageCoroutine = null;
    }
}
