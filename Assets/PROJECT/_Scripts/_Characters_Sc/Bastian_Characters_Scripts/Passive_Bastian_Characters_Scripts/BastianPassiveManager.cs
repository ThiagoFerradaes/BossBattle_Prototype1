using System;
using System.Collections;
using System.Threading;
using UnityEngine;


public class BastianPassiveManager : PassiveSkillManager {

    // Singleton
    public static BastianPassiveManager Instance;

    // Atributes
    float _currentHeatValue;
    bool _isLoosingAllHeat, _canLooseHeat;
    BastianHeatArea _currentHeatZone = BastianHeatArea.CoolArea;
    [HideInInspector] public bool CanShoot = true;

    // Components
    StatusManager _statusManager;
    BastianPassiveSO _info;
    HealthManager _healthManager;

    // Corrotines
    Coroutine _heatLostCoroutine, _looseAllHeatCoroutine, _looseHealthCoroutine;
    WaitForSeconds _healthLostCooldown, _heatLostCooldown;

    // Actions
    public event Action<float, float> OnHeatGain;
    public event Action<BastianHeatArea> OnHeatAreaChange;

    #region Initialize
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

    public override void OnStart(PassiveSO skill, GameObject parent) {
        base.OnStart(skill, parent);

        // Atribuindo variáveis se possível
        if (_info == null) _info = skill as BastianPassiveSO;
        if (_statusManager == null) _statusManager = parent.GetComponent<StatusManager>();
        if (_healthManager == null) _healthManager = parent.GetComponent<HealthManager>();

        // Wait for seconds
        _healthLostCooldown ??= new(_info.HealthLostByHeatCooldown);
        _heatLostCooldown ??= new(_info.TimeToLooseHeat);

        gameObject.SetActive(true);

        SetCanLooseHeat(true);

        // Ligando corrotinas
        _heatLostCoroutine ??= StartCoroutine(HeatLostPerTime());
        _looseHealthCoroutine ??= StartCoroutine(LooseHealthOverTime());

        AditionalUIManager.Instance.InstantiateUI(_info.HeatCanvas);

        CanShoot = true;
    }
    #endregion

    #region SetHeat
    public void SetCanLooseHeat(bool canLooseHeat) => _canLooseHeat = canLooseHeat;
    public void SetHeatToAmount(float newHeatAmount) {
        _currentHeatValue = newHeatAmount;
    }
    public void GainHeat(float amountOfHeatValueGained) {

        if (amountOfHeatValueGained <= 0) return;

        _currentHeatValue += amountOfHeatValueGained;
        
        _currentHeatValue = Mathf.Min(_currentHeatValue, _info.MaxHeat);

        if (_currentHeatValue == _info.MaxHeat) {
            _looseAllHeatCoroutine ??= StartCoroutine(HeatLostAfterLastHit());
        }

        CheckHeat();

        OnHeatGain?.Invoke(_currentHeatValue, _info.MaxHeat);
    }

    public void LooseHeat(float amountOfHeat) {
        _currentHeatValue -= amountOfHeat;

        _currentHeatValue = Mathf.Max(_currentHeatValue, 0);

        CheckHeat();

        OnHeatGain?.Invoke(_currentHeatValue, _info.MaxHeat);
    }

    void LooseHealth() {
        float currentHealth = _healthManager.ReturnCurrentHealth();
        float maxHealth = _healthManager.ReturnMaxHealth();
        float healthMultiplier = _info.AmountOfHealthToLoosePerArea[_currentHeatZone];
        float healthToLoose = maxHealth * healthMultiplier;

        float damage = Mathf.Min(healthToLoose, Mathf.Max(0, currentHealth - 1));
        if (damage > 0) _healthManager.TakeDamage(damage, false, _info.LooseHealthSound);
    }
    #endregion

    #region CheckHeat
    public BastianHeatArea ReturnCurrentHeatArea() => _currentHeatZone;
    public void LooseAllHeat() => _currentHeatValue = 0;
    public float ReturnCurrentHeat() => _currentHeatValue;
    public bool ReturnMinHeat(BastianHeatArea minHeatArea) {
        return _currentHeatZone >= minHeatArea;
    }
    public bool ReturnMaxHeat(BastianHeatArea minHeatArea) {
        return _currentHeatZone <= minHeatArea;
    }
    void CheckHeat() {
        if (_currentHeatValue >= _info.AmountOfHeatToHitOverHeatArea) {
            EnterOverHeatArea();
        }
        else if (_currentHeatValue >= _info.AmountOfHeatToHitHeatArea) {
            EnterHeatArea();
        }
        else {
            EnterCoolArea();
        }
    }
    void EnterCoolArea() {

        switch (_currentHeatZone) {
            case BastianHeatArea.CoolArea:
                return;
            case BastianHeatArea.HeatArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainHeat, false);
                break;
            case BastianHeatArea.OverHeatArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainHeat, false);
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainOverHeat, false);
                break;

        }

        ChangeHeatArea(BastianHeatArea.CoolArea);
    }

    void EnterHeatArea() {

        switch (_currentHeatZone) {
            case BastianHeatArea.HeatArea:
                return;
            case BastianHeatArea.CoolArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainHeat, true);
                break;
            case BastianHeatArea.OverHeatArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainOverHeat, false);
                break;
        }

        ChangeHeatArea(BastianHeatArea.HeatArea);
    }

    void EnterOverHeatArea() {

        switch (_currentHeatZone) {
            case BastianHeatArea.OverHeatArea:
                return;
            case BastianHeatArea.HeatArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainOverHeat, true);
                break;
            case BastianHeatArea.CoolArea:
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainHeat, true);
                _statusManager.ChangeStatusMultiplier(StatusType.Attack, _info.AmountOfAttackGainOverHeat, true);
                break;
        }

        ChangeHeatArea(BastianHeatArea.OverHeatArea);
    }

    void ChangeHeatArea(BastianHeatArea newArea) {
        bool increasedHeat = _currentHeatZone < newArea;
        _currentHeatZone = newArea;
        OnHeatAreaChange?.Invoke(_currentHeatZone);

        int switchIndex = Mathf.Clamp((int)(_currentHeatZone - 1), 0, 5);

        if (!increasedHeat) return;

        AK.Wwise.Switch newSwitch = _info.HeatZoneSwitchs[switchIndex];
        newSwitch.SetValue(parent);
        _info.HeatZoneChangeSound.Post(parent);
    }
    #endregion

    #region HeatCoroutines
    IEnumerator HeatLostPerTime() {
        while (true) {
            yield return _heatLostCooldown;
            if (!_isLoosingAllHeat && _canLooseHeat) LooseHeat(_info.HeatLostPerTime);
        }
    }

    IEnumerator HeatLostAfterLastHit() {
        CanShoot = false;
        _isLoosingAllHeat = true;

        float timer = 0;
        float amountOfHeatPerSecond = _info.MaxHeat / _info.TimeToLooseAllHeat;

        while (timer < _info.TimeToLooseAllHeat) {
            timer += Time.deltaTime;
            float amountOfHeatToLoose = amountOfHeatPerSecond * Time.deltaTime;
            LooseHeat(amountOfHeatToLoose);
            yield return null;
        }

        _looseAllHeatCoroutine = null;
        _isLoosingAllHeat = false;
        CanShoot = true;
    }

    IEnumerator LooseHealthOverTime() {
        while (true) {
            LooseHealth();
            yield return _healthLostCooldown;
        }
    }

    #endregion

}
