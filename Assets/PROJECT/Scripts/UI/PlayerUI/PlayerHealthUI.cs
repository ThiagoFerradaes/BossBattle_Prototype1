using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Components")]
    [SerializeField] Image healthBar;

    [Header("Shield Components")]
    [SerializeField] Image shieldBar;

    // Components
    GameObject _player;
    HealthManager _healthManager;

    private void Start() {
        _player = PlayerManager.Instance.Player;

        _healthManager = _player.GetComponent<HealthManager>();

        _healthManager.OnHealthChanged -= UpdateHealthUI;
        _healthManager.OnShieldChanged -= UpdateShieldUI;
        _healthManager.OnHealthChanged += UpdateHealthUI;
        _healthManager.OnShieldChanged += UpdateShieldUI;

        UpdateHealthUI(1, 1);
        UpdateShieldUI(0, 1);
    }

    void UpdateHealthUI(float currentHealth, float maxHealth) {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    void UpdateShieldUI(float currentShield, float maxShield) {
        shieldBar.fillAmount = currentShield / maxShield;
    }

    private void OnDestroy() {
        _healthManager.OnHealthChanged -= UpdateHealthUI;
        _healthManager.OnShieldChanged -= UpdateShieldUI;
    }
}
