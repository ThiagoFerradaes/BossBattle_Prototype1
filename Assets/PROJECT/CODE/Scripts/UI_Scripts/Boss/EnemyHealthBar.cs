using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] HealthManager healthManager;
    [SerializeField] Image healthBar;
    [SerializeField] bool isWorldCanvas = true;
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
    }

    private void OnDestroy() {
        healthManager.OnHealthChanged -= UpdateHealthBar;
    }
}
