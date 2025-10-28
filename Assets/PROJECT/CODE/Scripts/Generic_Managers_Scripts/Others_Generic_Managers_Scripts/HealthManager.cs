using System;
using System.Collections;
using UnityEngine;

// Script responsible for the health of all characters in the game who receive damage.
// It is also responsible for the shield that the character receives.
[RequireComponent(typeof(StatusManager))]
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
    /// current / max 
    /// </summary>
    public event Action<float, float> OnHealthChanged, OnShieldChanged;

    /// <summary>
    /// damage
    /// </summary>
    public event Action<float> OnDamageTaken;

    public event Action OnDeath, OnRevive;

    // Coroutines
    Coroutine _shieldCoroutine;
    #endregion

    //DEBUGING OPTIONS FOR VFX - SAMUEL
    bool imortal = false; //keep this false unless you want to debug vfxs and not die

    #region Methods

    #region Initialize
    private void Awake() {
        _statusManager = GetComponent<StatusManager>();
    }

    private void Start() {
        _maxHealth = imortal ? 1000000f : _statusManager.ReturnStatusValue(StatusType.MaxHealth);
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

    /// <summary>
    /// Function to take damage. Pass in the damage and if it hits the current shield of the character
    /// </summary>
    /// <param name="damageTaken"></param>
    /// <param name="hitShield"></param>
    public void TakeDamage(float damageTaken, bool hitShield) {
        if (_isDead || !_canTakeDamage) return;
        if (!hitShield) {
            ChangeHealth(_currentHealth - damageTaken);
            OnDamageTaken?.Invoke(damageTaken);
        }
        else {
            bool isShielded = _currentShield > 0;

            if (isShielded) {
                if (_currentShield > damageTaken) ChangeShield(_currentShield - damageTaken);
                else {
                    float realDamage = -(_currentShield - damageTaken);
                    ChangeShield(0);
                    ChangeHealth(_currentHealth - realDamage);
                    OnDamageTaken?.Invoke(realDamage);
                }
            }
            else {
                ChangeHealth(_currentHealth - damageTaken);
                OnDamageTaken?.Invoke(damageTaken);
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

    /// <summary>
    /// Heal the character
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount) {
        ChangeHealth(_currentHealth + amount);
    }

    /// <summary>
    /// Reset booleans -> canTakeDamage and isDead, also heals the character to max and reset the shield to 0
    /// </summary>
    public void Revive()
    {
        SetCanTakeDamage();
        _isDead = false;

        ChangeHealth(_maxHealth);
        ChangeShield(0);

        OnRevive?.Invoke();
    }

    /// <summary>
    /// The object dies
    /// </summary>
    public void Die() {
        ChangeHealth(0);
    }
    #endregion

    #region Shield
    void ChangeShield(float newShield) {
        _currentShield = Mathf.Clamp(newShield, 0, _maxHealth * _maxShield);
        OnShieldChanged?.Invoke(_currentShield, _maxHealth * _maxShield);
    }

    /// <summary>
    /// Recieve the amount of shield for a set duration
    /// </summary>
    /// <param name="shieldAmount"></param>
    /// <param name="shieldDuration"></param>
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

    /// <summary>
    /// Change the shield to 0
    /// </summary>
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
