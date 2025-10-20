using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Components")]
    [SerializeField] Image healthBar;

    [Header("Shield Components")]
    [SerializeField] Image shieldBar;

    [Header("Energy Components")]
    [SerializeField] Image energyBar;

    // Components
    GameObject _player;
    HealthManager _healthManager;

    // Actions
    Action<float, float> _healthChangeAction, _shieldChangeAction, _energyChangeAction;

    private void Start() {
        _player = PlayerManager.Instance.Player;

        _healthManager = _player.GetComponent<HealthManager>();

        _healthChangeAction = UpdateHealthUI;
        _shieldChangeAction = UpdateShieldUI;
        _energyChangeAction = UpdateEnergyUI;

        _healthManager.OnHealthChanged += _healthChangeAction;
        _healthManager.OnShieldChanged += _shieldChangeAction;
        EnergyManager.OnEnergyValueChanged += _energyChangeAction;

        UpdateHealthUI(1, 1);
        UpdateShieldUI(0, 1);
        UpdateEnergyUI(0, 1);
    }

    void UpdateHealthUI(float currentHealth, float maxHealth) {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    void UpdateShieldUI(float currentShield, float maxShield) {
        shieldBar.fillAmount = currentShield / maxShield;
    }

    void UpdateEnergyUI(float currentEnergy, float maxEnergy) {
        energyBar.fillAmount = currentEnergy / maxEnergy;
    }
    private void OnDestroy() {
        _healthManager.OnHealthChanged -= _healthChangeAction;
        _healthManager.OnShieldChanged -= _shieldChangeAction;
        EnergyManager.OnEnergyValueChanged -= _energyChangeAction;
    }
}
