using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] HealthManager healthManager;
    [SerializeField] Image healthBar;
    [SerializeField] Image damageBar;
    [SerializeField] float timeToStartDamageBar;
    [SerializeField] float damageBarDecreaseSpeed;
    [SerializeField] bool isWorldCanvas = true;
    [SerializeField] Renderer _renderer;

    [Header("Health Damage Indicator")]
    [SerializeField] int amountOfFlahses;
    [SerializeField] float flashDuration;
    [SerializeField] float timeBetweenFlashes;

    WaitForSeconds waitTimeBetweenFlashes, waitFlashDuration;

    private MaterialPropertyBlock _propBlock;

    Coroutine damageBarCoroutine;
    Coroutine changeColorTentacleCoroutine;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        healthManager.OnHealthChanged += UpdateHealthBar;
        healthManager.OnHealthChanged += ChangeColorOnHit;
        _propBlock = new MaterialPropertyBlock();

        SetWaitForSeconds();
    }

    void SetWaitForSeconds() {
        waitFlashDuration = new(flashDuration);
        waitTimeBetweenFlashes = new(timeBetweenFlashes);
    }

    void Update()
    {
        if (!isWorldCanvas) return;
        transform.LookAt(cam.transform.position);
    }

    void ChangeColorOnHit(float currentHealth, float maxHealth)
    {
        if(currentHealth != maxHealth) {
            changeColorTentacleCoroutine ??= StartCoroutine(ChangeColorTentacle());
        }
    }

    IEnumerator ChangeColorTentacle()
    {
        for (int i = 0; i < amountOfFlahses; i++) {

            // White
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_isHit", 1f);
            _renderer.SetPropertyBlock(_propBlock);

            yield return waitFlashDuration;

            // Normal
            _renderer.SetPropertyBlock(null);

            yield return waitTimeBetweenFlashes;
        }

        changeColorTentacleCoroutine = null;
    }

    void UpdateHealthBar(float currentHealth, float maxHealth) {

        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth == 0) {
            gameObject.SetActive(false);
            healthManager.OnHealthChanged -= UpdateHealthBar;
            return;
        }

        if (healthBar.fillAmount < 1) {
            if (!gameObject.activeInHierarchy) return;

            damageBarCoroutine ??= StartCoroutine(UpdateDamageBar());
        }
    }

    IEnumerator UpdateDamageBar() {
        yield return new WaitForSeconds(timeToStartDamageBar);

        while (damageBar.fillAmount > healthBar.fillAmount) {
            damageBar.fillAmount -= damageBarDecreaseSpeed;
            yield return null;
        }
        damageBar.fillAmount = healthBar.fillAmount;
        damageBarCoroutine = null;
    }

    private void OnDisable() {
        if (damageBarCoroutine != null) {
            StopCoroutine(damageBarCoroutine);
            damageBar.fillAmount = healthBar.fillAmount;
            damageBarCoroutine = null;
        }
        if (changeColorTentacleCoroutine != null) {
            StopCoroutine(changeColorTentacleCoroutine);
            _renderer.SetPropertyBlock(null);
        }
    }
    private void OnDestroy() {
        healthManager.OnHealthChanged -= UpdateHealthBar;
        healthManager.OnHealthChanged -= ChangeColorOnHit;
    }
}
