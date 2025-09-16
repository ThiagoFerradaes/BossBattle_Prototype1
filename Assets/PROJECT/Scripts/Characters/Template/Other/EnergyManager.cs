using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    // Atributes
    [SerializeField] float timeToGainEnergy;
    [SerializeField] float percentOfMaxEnergyToGainOverTime;
    float _currentEnergy;
    float _maxEnergy;
    bool _canGainEnergy = true;
    private StatusManager _statusManager;
    //private Coroutine _energyOverTimeCoroutine;

    // Lists
    private Action<Dictionary<SkillSlot, SkillSO>> _setMaxEnergy;

    // Actions
    public static event Action<float, float> OnEnergyValueChanged;

    #region Initialize
    private void Awake() {
        _statusManager = GetComponent<StatusManager>();

        _setMaxEnergy = (Dictionary<SkillSlot, SkillSO> skills) => SetMaxEnergy(skills);

        PlayerSkillManager.OnSkillsSet += _setMaxEnergy;
    }

    //private void Start() {
    //    if (_energyOverTimeCoroutine == null) {
    //        _energyOverTimeCoroutine = StartCoroutine(EnergyGainPerTime());
    //    }
    //    else {
    //        StopCoroutine(_energyOverTimeCoroutine);
    //        _energyOverTimeCoroutine = StartCoroutine(EnergyGainPerTime());
    //    }
    //}

    #endregion

    #region Energy
    void SetMaxEnergy(Dictionary<SkillSlot, SkillSO> skills) {
        if (!skills.ContainsKey(SkillSlot.Ultimate)) return;

        if (skills[SkillSlot.Ultimate] == null) return;

        UltimateSkillSO ultimate = skills[SkillSlot.Ultimate] as UltimateSkillSO;
        _maxEnergy = ultimate.EnergyCost;

        PlayerSkillManager.OnSkillsSet -= _setMaxEnergy;
    }

    public void GainEnergy(float energyAmount) {
        if (!_canGainEnergy) return;

        float energyRecharge = _statusManager.ReturnStatusValue(StatusType.EnergyRecharge)/100;
        float energyGain = energyAmount * energyRecharge;

        _currentEnergy += energyGain;
        _currentEnergy = Mathf.Min(_currentEnergy, _maxEnergy);
        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    public void LooseFlatEnergy(float energyFlatToLoose) {
        _currentEnergy -= energyFlatToLoose;

        _currentEnergy = Mathf.Max(_currentEnergy, 0);

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    public void LoosePercentEnergy(float percentOfEnergyToLoose) {
        float energyToLoose = _currentEnergy * (percentOfEnergyToLoose / 100);  

        _currentEnergy -= energyToLoose;

        _currentEnergy = Mathf.Max(_currentEnergy, 0);

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    public void LooseAllEnergy() {
        _currentEnergy = 0;

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    public bool HasFullEnergy() {
        return _currentEnergy >= _maxEnergy;
    }

    //IEnumerator EnergyGainPerTime() {
    //    float flatEnergyToGain = _maxEnergy * (percentOfMaxEnergyToGainOverTime / 100);
    //    while (true) {
    //        yield return new WaitForSeconds(timeToGainEnergy);
    //        float energyRecharge = _statusManager.ReturnStatusValue(StatusType.EnergyRecharge)/100;
    //        flatEnergyToGain *= energyRecharge;

    //        GainEnergy(flatEnergyToGain);
    //    }
    //}
    #endregion

    #region Setter

    public void SetCanGainEnergy(bool canGainEnergy) => _canGainEnergy = canGainEnergy;

    #endregion
}
