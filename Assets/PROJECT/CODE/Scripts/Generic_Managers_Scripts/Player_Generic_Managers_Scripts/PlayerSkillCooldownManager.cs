using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCooldownManager : MonoBehaviour {
    #region Parameter

    // Dictionaries
    private Dictionary<SkillSlot, float> _cooldowns = new();
    private Dictionary<SkillSlot, float> _MaxCooldowns = new();
    private Dictionary<SkillSlot, Coroutine> _runningCoroutines = new();
    private Dictionary<SkillSlot, int> _chargesDictionary = new();
    private Dictionary<SkillSlot, bool> _cooldownChargeDictionary = new();

    // Events
    public static event Action<SkillSlot, float> OnCooldownSet;
    public static event Action<SkillSlot, int> OnChargesSet;
    public static event Action<SkillSlot, int> OnChargesChange;

    // Actions 
    Action<Dictionary<SkillSlot, SkillSO>> _setCharges;

    #endregion

    #region Methods
    #region Initialize
    public void Initialize() {
        _setCharges = SetCharges;

        GetComponent<PlayerSkillManager>().OnSkillsSet -= _setCharges;
        GetComponent<PlayerSkillManager>().OnSkillsSet += _setCharges;

    }

    private void OnDisable() {
        OnCooldownSet = null;
        OnChargesSet = null;
        OnChargesChange = null;
        GetComponent<PlayerSkillManager>().OnSkillsSet -= _setCharges;
    }

    #endregion

    void SetCharges(Dictionary<SkillSlot, SkillSO> skills) {
        foreach (var skill in skills) {
            if (skill.Value is CommonSkillSO commonSkill) {
                _chargesDictionary[skill.Key] = commonSkill.Charges;
                _cooldownChargeDictionary[skill.Key] = false;
                _cooldowns[skill.Key] = 0f;
                _runningCoroutines[skill.Key] = null;

                OnChargesSet?.Invoke(skill.Key, commonSkill.Charges);
            }
        }
    }

    #region CooldownLogic
    public void SetCooldownWithCharges(SkillSlot slot, CommonSkillSO skill) {
        _MaxCooldowns[slot] = skill.Cooldown;

        _chargesDictionary[slot] = Mathf.Max(0, _chargesDictionary[slot] - 1);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);

        _cooldownChargeDictionary[slot] = true;
        StartCoroutine(CooldownBetweenChargesRoutine(slot, skill.ChargeCooldown));

        _runningCoroutines[slot] ??= StartCoroutine(CooldownCoroutine(slot, skill.Charges));

    }
    public void SetCooldownWithCharges(SkillSlot slot, CommonSkillSO skill, float cooldown) {
        _MaxCooldowns[slot] = cooldown;

        _chargesDictionary[slot] = Mathf.Max(0, _chargesDictionary[slot] - 1);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);

        _cooldownChargeDictionary[slot] = true;
        StartCoroutine(CooldownBetweenChargesRoutine(slot, skill.ChargeCooldown));

        _runningCoroutines[slot] ??= StartCoroutine(CooldownCoroutine(slot, skill.Charges));

    }
    IEnumerator CooldownBetweenChargesRoutine(SkillSlot slot, float cooldown) {
        yield return new WaitForSeconds(cooldown);
        _cooldownChargeDictionary[slot] = false;
    }

    public void SetCooldownSingleCharge(SkillSlot slot, float cooldown) {

        _chargesDictionary[slot] = Mathf.Max(0, _chargesDictionary[slot] - 1);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);

        _MaxCooldowns[slot] = cooldown;
        _runningCoroutines[slot] ??= StartCoroutine(CooldownCoroutine(slot, 1));

    }

    private IEnumerator CooldownCoroutine(SkillSlot slot, int maxCharges) {
        OnCooldownSet?.Invoke(slot, _MaxCooldowns[slot]);
        _cooldowns[slot] = _MaxCooldowns[slot];

        while (_cooldowns[slot] > 0f) {
            _cooldowns[slot] -= Time.deltaTime;
            yield return null;
        }

        _cooldowns[slot] = 0f;
        _chargesDictionary[slot] = Mathf.Min(_chargesDictionary[slot] + 1, maxCharges);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);
        _runningCoroutines[slot] = null;

        if (_chargesDictionary[slot] < maxCharges) {
            _cooldowns[slot] = _MaxCooldowns[slot];
            _runningCoroutines[slot] = StartCoroutine(CooldownCoroutine(slot, maxCharges));
        }
    }

    public void ResetCooldown(SkillSlot slot) {
        if (_runningCoroutines.TryGetValue(slot, out Coroutine running) && running != null) {
            StopCoroutine(running);
            _runningCoroutines[slot] = null;
        }

        _cooldowns[slot] = 0f;
        _chargesDictionary[slot] = 1;
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);
        OnCooldownSet?.Invoke(slot, 0f);
    }


    public bool ReturnIfCanUseSkill(SkillSlot slot) {
        if (!_chargesDictionary.TryGetValue(slot, out int charges))
            return false;
        bool canUse = charges > 0f && _cooldownChargeDictionary[slot] == false;

        return canUse;
    }
    #endregion
    #endregion
}
