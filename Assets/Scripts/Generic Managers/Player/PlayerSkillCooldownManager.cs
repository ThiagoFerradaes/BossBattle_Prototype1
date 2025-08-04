using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCooldownManager : MonoBehaviour
{
    #region Parameter

    // Dictionaries
    private Dictionary<SkillSlot, float> _cooldowns = new();
    private Dictionary<SkillSlot, Coroutine> _runningCoroutines = new();

    // Events
    public static event Action<SkillSlot, float> OnCooldownSet;

    #endregion

    #region Methods
    private void Awake()
    {
        foreach (SkillSlot slot in System.Enum.GetValues(typeof(SkillSlot)))
        {
            _cooldowns[slot] = 0f;
        }
    }

    public void SetCooldown(SkillSlot slot, float cooldown)
    {
        if (_runningCoroutines.TryGetValue(slot, out Coroutine running) && running != null) {
            StopCoroutine(running);
        }

        _cooldowns[slot] = cooldown;
        _runningCoroutines[slot] = StartCoroutine(CooldownCoroutine(slot));

        if (slot != SkillSlot.BaseAttack) OnCooldownSet?.Invoke(slot, cooldown);
    }

    private IEnumerator CooldownCoroutine(SkillSlot slot)
    {
        while (_cooldowns[slot] > 0f)
        {
            _cooldowns[slot] -= Time.deltaTime;
            yield return null;
        }

        _cooldowns[slot] = 0f;
        _runningCoroutines.Remove(slot);
    }

    public void ResetCooldown(SkillSlot slot) {
        if (_runningCoroutines.TryGetValue(slot, out Coroutine running) && running != null) {
            StopCoroutine(running);
            _runningCoroutines.Remove(slot);
        }

        _cooldowns[slot] = 0f;
        OnCooldownSet?.Invoke(slot, 0f);
    }


    public float ReturnCooldown(SkillSlot slot) => _cooldowns[slot];
    #endregion
}
