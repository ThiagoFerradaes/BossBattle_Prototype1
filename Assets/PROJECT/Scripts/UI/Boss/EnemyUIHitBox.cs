using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHitBox : MonoBehaviour
{
    [SerializeField] HealthManager healthManager;
    [SerializeField] Image healthBar;
    Camera cam;
    void Start()
    {
        cam = Camera.main;
        healthManager.OnHealthChanged += UpdateHealthBar;
    }

    void Update()
    {
        transform.LookAt(cam.transform.position);
    }

    void UpdateHealthBar(float currentHealth, float maxHealth) {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
