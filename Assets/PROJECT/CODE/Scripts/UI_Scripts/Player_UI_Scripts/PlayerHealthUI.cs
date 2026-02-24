using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Components")]
    [SerializeField] Image healthBar;
    [SerializeField] Image damageBar;
    [SerializeField] float timeToStartDamageBar;
    [SerializeField] float damageBarDecreaseSpeed;

    [Header("Shield Components")]
    [SerializeField] Image shieldBar;

    [Header("Energy Components")]
    [SerializeField] Image energyBar;

    // Components
    GameObject _player;
    HealthManager _healthManager;

    // Actions
    Action<float, float> _healthChangeAction, _shieldChangeAction, _energyChangeAction;

    // Corrotinas
    Coroutine damageBarCoroutine;

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

    public void SetUi()
    {
        Start();
    }
    
    void UpdateHealthUI(float currentHealth, float maxHealth) {
        float oldHealth = healthBar.fillAmount;

        healthBar.fillAmount = currentHealth / maxHealth;

        if (healthBar.fillAmount < oldHealth) damageBarCoroutine ??= StartCoroutine(UpdateDamageBar());
        else {
            if (healthBar.fillAmount > damageBar.fillAmount) damageBar.fillAmount = healthBar.fillAmount;
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
