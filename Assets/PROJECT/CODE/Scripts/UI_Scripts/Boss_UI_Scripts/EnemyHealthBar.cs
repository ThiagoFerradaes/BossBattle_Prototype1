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

    Coroutine damageBarCoroutine;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        healthManager.OnHealthChanged += UpdateHealthBar;
    }

    void Update()
    {
        if (!isWorldCanvas) return;
        transform.LookAt(cam.transform.position);
    }

    void UpdateHealthBar(float currentHealth, float maxHealth) {
        healthBar.fillAmount = currentHealth / maxHealth;
        if (currentHealth == 0) gameObject.SetActive(false);
        if (healthBar.fillAmount < 1) damageBarCoroutine ??= StartCoroutine(UpdateDamageBar());
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
    }
    private void OnDestroy() {
        healthManager.OnHealthChanged -= UpdateHealthBar;
    }
}
