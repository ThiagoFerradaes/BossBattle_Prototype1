using System;
using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    #region Paramethers
    // floats
    float maxHealth;
    public float _currentHealth;
    float maxShield;
    private float _currentShield;

    // Components
    StatusManager _statusManager;

    // Events
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;
    public event Action OnDeath;
    #endregion

    #region Methods

    #region Initialize
    private void Awake() {
        _statusManager = GetComponent<StatusManager>();
        maxHealth = _statusManager.ReturnStatusValue(StatusType.MaxHealth);
        maxShield = _statusManager.ReturnStatusValue(StatusType.MaxAmountOfShield);
        ChangeHealth(maxHealth);
        ChangeShield(0);
    }
    #endregion

    #region Health
    void ChangeHealth(float newHealth) {
        _currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth == 0) OnDeath?.Invoke();
    }

    public void TakeDamage(float damage, bool trueDamage) {
        if (trueDamage) ChangeHealth(_currentHealth - damage);
        else {
            bool isShielded = _currentShield > 0;

            if (isShielded) {
                if (_currentShield > damage) ChangeShield(_currentShield - damage);
                else {
                    float realDamage = -(_currentShield - damage);
                    ChangeShield(0);
                    ChangeHealth(realDamage);
                }
            }
            else ChangeHealth(_currentHealth - damage);
        }
    }
    #endregion

    #region Shield
    void ChangeShield(float newShield) {
        _currentShield = Mathf.Clamp(newShield, 0, maxHealth * maxShield);
        OnShieldChanged?.Invoke(_currentShield, maxHealth * maxShield);
    }
    public void RecieveShield(float shieldAmount, float shieldDuration) {
        StartCoroutine(ShieldDuration(shieldAmount, shieldDuration));
    }

    IEnumerator ShieldDuration(float shieldAmount, float shieldDuration) {
        ChangeShield(shieldAmount);
        yield return new WaitForSeconds(shieldDuration);
        ChangeShield(_currentShield - shieldAmount);
    }

    #endregion

    #endregion
}
