using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerSkillCooldownManager : MonoBehaviour
{
    #region Parameter

    // Dictionaries
    private Dictionary<SkillSlot, float> _cooldowns = new();
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
    private void Awake()
    {

        _setCharges = SetCharges;
        PlayerSkillManager.OnSkillsSet += _setCharges;
    }

    private void OnDestroy()
    {
        OnCooldownSet = null;
        PlayerSkillManager.OnSkillsSet -= _setCharges;
    }

    #endregion

    void SetCharges(Dictionary<SkillSlot, SkillSO> skills)
    {
        foreach (var skill in skills)
        {
            if (skill.Value is CommonSkillSO commonSkill)
            {
                _chargesDictionary[skill.Key] = commonSkill.Charges;
                _cooldownChargeDictionary[skill.Key] = false;
                _cooldowns[skill.Key] = 0f;
                _runningCoroutines[skill.Key] = null;

                OnChargesSet?.Invoke(skill.Key, commonSkill.Charges);
            }
        }
    }

    #region CooldownLogic
    public void SetCooldownWithCharges(SkillSlot slot, CommonSkillSO skill)
    {

        _chargesDictionary[slot]--;
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);

        _cooldownChargeDictionary[slot] = true;
        StartCoroutine(CooldownBetweenChargesRoutine(slot, skill.ChargeCooldown));

        _cooldowns[slot] = skill.Cooldown;
        _runningCoroutines[slot] ??= StartCoroutine(CooldownCoroutine(slot, skill.Charges));

        if (slot != SkillSlot.BaseAttack) OnCooldownSet?.Invoke(slot, skill.Cooldown);
    }

    IEnumerator CooldownBetweenChargesRoutine(SkillSlot slot, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _cooldownChargeDictionary[slot] = false;
    }

    public void SetCooldownSingleCharge(SkillSlot slot, float cooldown)
    {

        _chargesDictionary[slot] = Mathf.Max(0, _chargesDictionary[slot] - 1);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);

        _cooldowns[slot] = cooldown;
        _runningCoroutines[slot] ??= StartCoroutine(CooldownCoroutine(slot, 1));

        if (slot != SkillSlot.BaseAttack) OnCooldownSet?.Invoke(slot, cooldown);
    }

    private IEnumerator CooldownCoroutine(SkillSlot slot, int maxCharges)
    {
        while (_cooldowns[slot] > 0f)
        {
            _cooldowns[slot] -= Time.deltaTime;
            yield return null;
        }

        _cooldowns[slot] = 0f;
        _chargesDictionary[slot] = Mathf.Min(_chargesDictionary[slot] + 1, maxCharges);
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);
        _runningCoroutines[slot] = null;
    }

    public void ResetCooldown(SkillSlot slot)
    {
        if (_runningCoroutines.TryGetValue(slot, out Coroutine running) && running != null)
        {
            StopCoroutine(running);
            _runningCoroutines[slot] = null;
        }

        _cooldowns[slot] = 0f;
        _chargesDictionary[slot] = 1;
        OnChargesChange?.Invoke(slot, _chargesDictionary[slot]);
        OnCooldownSet?.Invoke(slot, 0f);
    }


    public bool ReturnIfCanUseSkill(SkillSlot slot)
    {
        if (!_chargesDictionary.TryGetValue(slot, out int charges))
            return false;
        bool canUse = charges > 0f && _cooldownChargeDictionary[slot] == false;

        return canUse;
    }
    #endregion
    #endregion
}
