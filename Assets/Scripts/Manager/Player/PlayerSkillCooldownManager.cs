using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCooldownManager : MonoBehaviour
{
    private Dictionary<SkillSlot, float> _cooldowns = new();
    private Dictionary<SkillSlot, Coroutine> _runningCoroutines = new();

    private void Awake()
    {
        foreach (SkillSlot slot in System.Enum.GetValues(typeof(SkillSlot)))
        {
            _cooldowns[slot] = 0f;
        }
    }

    public void SetCooldown(SkillSlot slot, float cooldown)
    {
        if (_runningCoroutines.TryGetValue(slot, out Coroutine running))
        {
            StopCoroutine(running);
        }

        _cooldowns[slot] = cooldown;
        _runningCoroutines[slot] = StartCoroutine(CooldownCoroutine(slot));
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
        Debug.Log($"{slot} cooldown ended");
    }

    public float ReturnCooldown(SkillSlot slot) => _cooldowns[slot];
}
