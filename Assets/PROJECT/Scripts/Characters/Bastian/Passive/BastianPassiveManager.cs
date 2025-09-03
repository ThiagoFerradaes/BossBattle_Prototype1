using System;
using System.Collections;
using UnityEngine;

public class BastianPassiveManager : PassiveSkillManager {

    public enum HeatArea { CoolArea, HeatArea, SuperHeatArea, OverHeatArea, LastOverHeatArea }

    public static BastianPassiveManager Instance;

    float _currentHeat;
    StatusManager _statusManager;

    BastianPassiveSO _info;

    Coroutine _heatLostCoroutine;

    HeatArea _heatArea = HeatArea.CoolArea;

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

        _info = skill as BastianPassiveSO;

        gameObject.SetActive(true);

        if (_heatLostCoroutine == null) {
            _heatLostCoroutine = StartCoroutine(HeatLostPerTime());
        }
        else {
            StopCoroutine(_heatLostCoroutine);
            _heatLostCoroutine = StartCoroutine(HeatLostPerTime());
        }

        Instantiate(_info.HeatCanvas);
    }

    public void GainHeat(float amountOfHeat) {
        _currentHeat += amountOfHeat;

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

        _heatArea = HeatArea.CoolArea;
        _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGain, false);
    }

    void EnterHeatArea() {
        if (_heatArea != HeatArea.CoolArea) return;

        _heatArea = HeatArea.HeatArea;
        _statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AmountOfAttackSpeedGain, true);
    }

    void EnterSuperHeatArea() {
        _heatArea = HeatArea.SuperHeatArea;
    }

    void EnterOverHeatArea() {
        _heatArea = HeatArea.OverHeatArea;
    }

    void LastOverHeatHit() {
        _heatArea = HeatArea.LastOverHeatArea;
    }

    IEnumerator HeatLostPerTime() {
        while (true) {
            yield return new WaitForSeconds(_info.TimeToLooseHeat);
            LooseHeat(_info.HeatLostPerTime);
        }
    }
}
