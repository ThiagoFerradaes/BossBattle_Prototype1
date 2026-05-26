using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {
    [Header("Health Components")]
    [SerializeField] Image healthBar;
    [SerializeField] Image damageBar;
    [SerializeField] float timeToStartDamageBar;
    [SerializeField] float damageBarDecreaseSpeed;

    [Header("Shield Components")]
    [SerializeField] Image shieldBar;

    [Header("Energy Components")]
    [SerializeField] Image energyBar;
    [SerializeField] Image ultimateImage;
    [SerializeField] GameObject ultimateReadyFlashVFX;
    [SerializeField] Color ultimateCanvasGroupNoUltimateColor;
    [SerializeField] Color ultimateCanvasGroupUltimateReadyColor;
    [SerializeField] float ultimateReadyFlashVFXDuration;
    [SerializeField] AK.Wwise.Event soundWhenEnergyAtMax;

    [Header("AnimatedBackgrounds")]
    [SerializeField] GameObject cyrusAnimatedBackground;
    [SerializeField] GameObject bastianAnimatedBackground;

    // Components
    GameObject _player;
    HealthManager _healthManager;

    // Actions
    Action<float, float> _healthChangeAction, _shieldChangeAction, _energyChangeAction;

    // Corrotinas
    Coroutine damageBarCoroutine, flashCoroutine;

    WaitForSeconds flashWaitForSeconds;

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

        flashWaitForSeconds = new(ultimateReadyFlashVFXDuration);
    }

    private void OnDestroy() {
        _healthManager.OnHealthChanged -= _healthChangeAction;
        _healthManager.OnShieldChanged -= _shieldChangeAction;
        EnergyManager.OnEnergyValueChanged -= _energyChangeAction;
    }

    public void SetUi() {
        Start();
    }

    #region Health Region
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

    #endregion

    #region Energy Region
    void UpdateEnergyUI(float currentEnergy, float maxEnergy) {
        energyBar.fillAmount = currentEnergy / maxEnergy;

        HandleAnimatedBackground(currentEnergy == maxEnergy);
    }

    void HandleAnimatedBackground(bool isOn) {
        if (!isOn) {
            cyrusAnimatedBackground.SetActive(false);
            bastianAnimatedBackground.SetActive(false);
            ultimateImage.color = ultimateCanvasGroupNoUltimateColor;
            return;
        }

        Character currentCharacter = CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacter();

        switch (currentCharacter) {
            case Character.Cyrus: cyrusAnimatedBackground.SetActive(true); break;
            case Character.Bastian: bastianAnimatedBackground.SetActive(true); break;
        }

        ultimateImage.color = ultimateCanvasGroupUltimateReadyColor;

        flashCoroutine ??= StartCoroutine(UltimateReadyFlashVFXCoroutine());

        soundWhenEnergyAtMax.Post(gameObject);
    }

    IEnumerator UltimateReadyFlashVFXCoroutine() {
        ultimateReadyFlashVFX.SetActive(true);
        yield return flashWaitForSeconds;
        ultimateReadyFlashVFX.SetActive(false);

        flashCoroutine = null;
    }

    #endregion
}
