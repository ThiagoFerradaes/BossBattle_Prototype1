using System;
using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour {
    // floats
    float maxHealth;
    private float _currentHealth;
    float maxShield;
    private float _currentShield;

    // Components
    StatusManager _statusManager;

    // Events
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;

    #region Methods

    private void Awake() {
        _statusManager = GetComponent<StatusManager>();
        maxHealth = _statusManager.ReturnStatusValue(StatusType.MaxHealth);
        maxShield = _statusManager.ReturnStatusValue(StatusType.MaxAmountOfShield);
        ChangeHealth(maxHealth);
        ChangeSheild(0);
    }

    void ChangeHealth(float newHealth) {
        _currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    void ChangeSheild(float newShield) {
        _currentShield = Mathf.Clamp(newShield, 0, maxHealth * maxShield);
        OnShieldChanged?.Invoke(_currentShield, maxHealth * maxShield);
    }

    public void TakeDamage(float damage, bool trueDamage) {
        if (trueDamage) ChangeHealth(_currentHealth - damage);
        else {
            bool isShielded = _currentShield > 0;

            if (isShielded) {
                if (_currentShield > damage) ChangeSheild(_currentShield -  damage);
                else {
                    float realDamage = -(_currentShield - damage);
                    ChangeSheild(0);
                    ChangeHealth(realDamage);
                }
            }
        }
    }

    public void RecieveShield(float shieldAmount, float shieldDuration) {
        StartCoroutine(ShieldDuration(shieldAmount, shieldDuration));
    }

    IEnumerator ShieldDuration(float shieldAmount, float shieldDuration) {
        ChangeSheild(shieldAmount);
        yield return new WaitForSeconds(shieldDuration);
        ChangeSheild(_currentShield - shieldAmount);
    }
    #endregion
}
