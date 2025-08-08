using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCooldownManager : MonoBehaviour {
    public static EnemyCooldownManager Instance;
    Dictionary<EnemySkillSO, float> _listOfCooldowns = new();

    private void Awake() {
        if (Instance == null) { Instance = this; }
        else Destroy(this);
    }
    public void Initiate(List<EnemySkillSO> list) {
        foreach (EnemySkillSO item in list) {
            _listOfCooldowns[item] = 0f;
        }
    }

    public bool SkillInCooldown(EnemySkillSO skill) {
        if (_listOfCooldowns.ContainsKey(skill)) {
            if (_listOfCooldowns[skill] > 0f) return true;
            else return false;
        }
        else return true;
    }

    public void SetSkillCooldown(EnemySkillSO skill) {
        if (!_listOfCooldowns.ContainsKey(skill)) return;

        _listOfCooldowns[skill] = skill.Cooldown;
        StartCoroutine(SkillCooldown(skill));
    }

    IEnumerator SkillCooldown(EnemySkillSO skill) {

        while (_listOfCooldowns[skill] > 0) {
            _listOfCooldowns[skill] -= Time.deltaTime;
            yield return null;
        }

        _listOfCooldowns[skill] = 0f;
    }
}
