using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public enum HeatArea { CoolArea = 0, HeatArea = 1, SuperHeatArea = 2, OverHeatArea = 3, LastOverHeatArea = 4 }
public class BastianPassiveManager : PassiveSkillManager {

    // Singleton
    public static BastianPassiveManager Instance;

    // Atributes
    float _currentHeat;
    bool _looseAllHeat;
    HeatArea _heatArea = HeatArea.CoolArea;
    [HideInInspector] public bool CanShoot = true;

    // Components
    StatusManager _statusManager;
    BastianPassiveSO _info;
    HealthManager _healthManager;

    // Corrotines
    Coroutine _heatLostCoroutine, _looseAllHeatCoroutine, _looseHealthCoroutine;

    // Actions
    public event Action<float, float> OnHeatGain;

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

        if (_info == null) _info = skill as BastianPassiveSO;
        if (_statusManager == null) _statusManager = parent.GetComponent<StatusManager>();
        if (_healthManager == null) _healthManager = parent.GetComponent<HealthManager>();

        gameObject.SetActive(true);

        _heatLostCoroutine ??= StartCoroutine(HeatLostPerTime());

        AditionalUIManager.Instance.InstantiateUI(_info.HeatCanvas);

        CanShoot = true;
    }

    public void GainHeat(float amountOfHeat) {
        if (_currentHeat + amountOfHeat <= _info.HeatToHitOverHeatArea)
            _currentHeat += amountOfHeat;
        else if (_currentHeat < _info.HeatToHitOverHeatArea) {
            _currentHeat = _info.HeatToHitOverHeatArea;
        }
        else {
            _currentHeat++;
        }

        if (_currentHeat == _info.MaxHeat) {
            _looseAllHeat = true;
            _looseAllHeatCoroutine ??= StartCoroutine(HeatLostAfterLastHit());
        }

        _currentHeat = Mathf.Min(_currentHeat, _info.MaxHeat);

        CheckHeat();

        OnHeatGain?.Invoke(_currentHeat, _info.MaxHeat);
    }

    public void LooseHeat(float amountOfHeat) {
        _currentHeat -= amountOfHeat;

        _currentHeat = Mathf.Max(_currentHeat, 0);

        CheckHeat();

        OnHeatGain?.Invoke(_currentHeat, _info.MaxHeat);
    }

    void LooseHealth() {
        float healthToLoose = _healthManager.ReturnMaxHealth() * _info.PercentOfMaxHealthLostPerTime / 100;
        if(_healthManager.ReturnCurrentHealth() > 1)_healthManager.TakeDamage(healthToLoose, false);
    }

    #region CheckHeat
    void CheckHeat() {
        if (_currentHeat >= _info.HeatToHitLastOverHeatArea) {
            LastOverHeatHit();
        }
        else if (_currentHeat >= _info.HeatToHitOverHeatArea) {
            EnterOverHeatArea();
        }
        else if (_currentHeat >= _info.HeatToHitSuperHeatArea) {
            EnterSuperHeatArea();
        }
        else if (_currentHeat >= _info.HeatToHitHeatArea) {
            EnterHeatArea();
        }
        else {
            EnterCoolArea();
        }
    }
    void EnterCoolArea() {
        if (_heatArea == HeatArea.CoolArea) return;

        if (_heatArea == HeatArea.HeatArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainHeat, false);
        }
        else if (_heatArea >= HeatArea.HeatArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainSuperHeat, false);
        }

        _heatArea = HeatArea.CoolArea;
    }

    void EnterHeatArea() {
        if (_heatArea == HeatArea.HeatArea) return;

        if (_heatArea == HeatArea.CoolArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainHeat, true);
        }
        else if (_heatArea >= HeatArea.SuperHeatArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainSuperHeat, false);
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainHeat, true);
        }

        _heatArea = HeatArea.HeatArea;
    }

    void EnterSuperHeatArea() {
        if (_heatArea == HeatArea.SuperHeatArea) return;

        if (_heatArea == HeatArea.CoolArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainSuperHeat, true);
        }
        else if (_heatArea == HeatArea.HeatArea) {
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainHeat, false);
            _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGainSuperHeat, true);
        }

        _heatArea = HeatArea.SuperHeatArea;
        if(!_looseAllHeat) _looseHealthCoroutine ??= StartCoroutine(LooseHealthOverTime());
    }

    void EnterOverHeatArea() {
        if (_heatArea == HeatArea.OverHeatArea) return;

        _heatArea = HeatArea.OverHeatArea;
    }

    void LastOverHeatHit() {
        if (_heatArea == HeatArea.LastOverHeatArea) return;

        _heatArea = HeatArea.LastOverHeatArea;
    }
    #endregion
    IEnumerator HeatLostPerTime() {
        while (true) {
            yield return new WaitForSeconds(_info.TimeToLooseHeat);
            if (_currentHeat < _info.HeatToHitOverHeatArea || _looseAllHeat) LooseHeat(_info.HeatLostPerTime);
        }
    }

    IEnumerator HeatLostAfterLastHit() {
        CanShoot = false;

        float timer = 0;
        float amountOfHeatPerSecond = _info.MaxHeat/_info.TimeToLooseAllHeatAfterLastHit;

        while (timer < _info.TimeToLooseAllHeatAfterLastHit) {
            timer += Time.deltaTime;
            float amountOfHeatToLoose = amountOfHeatPerSecond * Time.deltaTime;
            LooseHeat(amountOfHeatToLoose);
            yield return null;
        }

        _looseAllHeatCoroutine = null;
        _looseAllHeat = false;
        CanShoot = true;
    }

    IEnumerator LooseHealthOverTime() {
        while (_heatArea == _info.AreaToLooseHealth) {
            LooseHealth();
            yield return new WaitForSeconds(_info.TimeToLooseHealth);
        }

        _looseHealthCoroutine = null;
    }

    public bool ReturnMinHeat(HeatArea minHeatArea) {
        return _heatArea >= minHeatArea;
    }
    public bool ReturnMaxHeat(HeatArea minHeatArea)
    {
        return _heatArea <= minHeatArea;
    }

}
