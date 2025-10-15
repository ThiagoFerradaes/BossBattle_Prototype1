using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HealthManager : MonoBehaviour {

    #region Paramethers
    // floats
    float _maxHealth;
    float _currentHealth;
    float _maxShield;
    float _currentShield;

    // Components
    StatusManager _statusManager;

    // Bools 
    bool _isDead;
    bool _canTakeDamage = true;

    // Events
    /// <summary>
    /// current health / max health
    /// </summary>
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;
    public event Action<float> OnDamageTaken;
    public event Action OnDeath;
    public event Action OnRevive;

    // Coroutines
    Coroutine _shieldCoroutine;
    #endregion

    #region Methods

    #region Initialize
    private void Awake() {
        _statusManager = GetComponent<StatusManager>();
    }

    private void Start() {
        _maxHealth = _statusManager.ReturnStatusValue(StatusType.MaxHealth);
        _maxShield = _statusManager.ReturnStatusValue(StatusType.MaxAmountOfShield)/100;
        ChangeHealth(_maxHealth);
        ChangeShield(0);
    }
    #endregion

    #region Health
    void ChangeHealth(float newHealth) {
        _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0) {
            _isDead = true;
            OnDeath?.Invoke();
        }
        else {
            _isDead = false;
        }
    }

    public void TakeDamage(float damage, bool hitShield) {
        if (_isDead || !_canTakeDamage) return;
        if (!hitShield) {
            ChangeHealth(_currentHealth - damage);
            OnDamageTaken?.Invoke(damage);
        }
        else {
            bool isShielded = _currentShield > 0;

            if (isShielded) {
                if (_currentShield > damage) ChangeShield(_currentShield - damage);
                else {
                    float realDamage = -(_currentShield - damage);
                    ChangeShield(0);
                    ChangeHealth(_currentHealth - realDamage);
                    OnDamageTaken?.Invoke(realDamage);
                }
            }
            else {
                ChangeHealth(_currentHealth - damage);
                OnDamageTaken?.Invoke(damage);
            }
        }
    }

    /// <summary>
    /// The character will no longer take damage
    /// </summary>
    public void SetCantTakeDamage() => _canTakeDamage = false;
    /// <summary>
    /// The character will be able to take damage
    /// </summary>
    public void SetCanTakeDamage() => _canTakeDamage = true;

    public bool ReturnIfCanTakeDamage() {
        return _canTakeDamage && !_isDead;
    }

    public void Heal(float amount) {
        ChangeHealth(_currentHealth + amount);
    }

    public void Revive()
    {
        _canTakeDamage = true;
        _isDead = false;

        ChangeHealth(_maxHealth);
        ChangeShield(0);

        OnRevive?.Invoke();
    }
    #endregion

    #region Shield
    void ChangeShield(float newShield) {
        _currentShield = Mathf.Clamp(newShield, 0, _maxHealth * _maxShield);
        OnShieldChanged?.Invoke(_currentShield, _maxHealth * _maxShield);
    }
    public void RecieveShield(float shieldAmount, float shieldDuration) {
        if (_shieldCoroutine == null)
            _shieldCoroutine = StartCoroutine(ShieldDuration(shieldAmount, shieldDuration));
        else {
            StopCoroutine(_shieldCoroutine);
            _shieldCoroutine = StartCoroutine(ShieldDuration(shieldAmount, shieldDuration));
        }
    }

    IEnumerator ShieldDuration(float shieldAmount, float shieldDuration) {

        float increasedShield = _currentShield + shieldAmount;
        ChangeShield(increasedShield);

        float timer = 0f;
        float shiledLostPerSecond = increasedShield / shieldDuration;

        while (timer < shieldDuration && _currentShield > 0) {
            timer += Time.deltaTime;
            float newShield = _currentShield - (shiledLostPerSecond * Time.deltaTime);
            ChangeShield(newShield);
            yield return null;
        }

        BreakShield();

        _shieldCoroutine = null;
    }

    public void BreakShield() {
        ChangeShield(0);
    }

    #endregion

    #region Getters

    public float ReturnMaxHealth() => _maxHealth;

    public float ReturnCurrentHealth() => _currentHealth;

    public bool ReturnIfIsDead() => _isDead;
    #endregion

    #endregion
}
