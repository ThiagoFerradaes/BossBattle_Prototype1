using System;
using System.Collections.Generic;
using UnityEngine;

// Script responsable for the energy of the playable characters
[RequireComponent(typeof(StatusManager))]
public class EnergyManager : MonoBehaviour
{
    // Atributes
    float _currentEnergy;
    float _maxEnergy;
    bool _canGainEnergy = true;
    StatusManager _statusManager;

    // Lists
    Action<Dictionary<SkillSlot, SkillSO>> _setMaxEnergy;

    // Actions
    public static event Action<float, float> OnEnergyValueChanged;

    #region Initialize
    public void Initialize(GameObject player) {
        _statusManager = GetComponent<StatusManager>();

        _setMaxEnergy = (Dictionary<SkillSlot, SkillSO> skills) => SetMaxEnergy(skills, player);

        player.GetComponent<PlayerSkillManager>().OnSkillsSet -= _setMaxEnergy;
        player.GetComponent<PlayerSkillManager>().OnSkillsSet += _setMaxEnergy;
    }

    private void OnDisable() {
        OnEnergyValueChanged = null;
    }
    #endregion

    #region Energy
    void SetMaxEnergy(Dictionary<SkillSlot, SkillSO> skills, GameObject player) {
        if (!skills.ContainsKey(SkillSlot.Ultimate)) return;

        if (skills[SkillSlot.Ultimate] == null) return;

        UltimateSkillSO ultimate = skills[SkillSlot.Ultimate] as UltimateSkillSO;
        _maxEnergy = ultimate.EnergyCost;

        player.GetComponent<PlayerSkillManager>().OnSkillsSet -= _setMaxEnergy;
    }

    public void ChangeMaxEnergy(float newMaxEnergyValue) {
        _maxEnergy = newMaxEnergyValue;
    }

    /// <summary>
    /// The character recieve an amount of energy
    /// </summary>
    /// <param name="energyAmount"></param>
    public void GainEnergy(float energyAmount) {
        if (!_canGainEnergy) return;

        float energyRecharge = _statusManager.ReturnStatusValue(StatusType.EnergyRecharge);
        float energyGain = energyAmount * energyRecharge;

        _currentEnergy += energyGain;
        _currentEnergy = Mathf.Min(_currentEnergy, _maxEnergy);
        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    /// <summary>
    /// The character loose the amount of energy
    /// </summary>
    /// <param name="energyFlatToLoose"></param>
    public void LooseFlatEnergy(float energyFlatToLoose) {
        _currentEnergy -= energyFlatToLoose;

        _currentEnergy = Mathf.Max(_currentEnergy, 0);

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    /// <summary>
    /// The character loose the percent of energy. Percent scale (0 - 100)
    /// </summary>
    /// <param name="percentOfEnergyToLoose"></param>
    public void LoosePercentEnergy(float percentOfEnergyToLoose) {
        float energyToLoose = _currentEnergy * (percentOfEnergyToLoose / 100);  

        _currentEnergy -= energyToLoose;

        _currentEnergy = Mathf.Max(_currentEnergy, 0);

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }

    /// <summary>
    /// The character looses all his energy
    /// </summary>
    public void LooseAllEnergy() {
        _currentEnergy = 0;

        OnEnergyValueChanged?.Invoke(_currentEnergy, _maxEnergy);
    }


    /// <summary>
    /// Return if the character has full energy
    /// </summary>
    /// <returns></returns>
    public bool HasFullEnergy() {
        return _currentEnergy >= _maxEnergy;
    }

    #endregion

    #region Setter

    /// <summary>
    /// Block (false) or unblock (true) the gain of energy of the character
    /// </summary>
    /// <param name="canGainEnergy"></param>
    public void SetCanGainEnergy(bool canGainEnergy) => _canGainEnergy = canGainEnergy;

    #endregion
}
