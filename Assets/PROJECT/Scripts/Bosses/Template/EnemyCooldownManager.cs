using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCooldownManager : MonoBehaviour {

    #region Parameters

    public static EnemyCooldownManager Instance;
    [SerializedDictionary("Skill", "Cooldown"), SerializeField] SerializedDictionary<EnemyBehaviourSO, float> _listOfCooldowns = new();

    #endregion

    #region Methods

    #region Initialize
    private void Awake() {
        if (Instance == null) { // Singleton
            Instance = this;
        }
        else Destroy(this);
    }
    public void Initiate(List<EnemyBehaviourSO> list) {
        foreach (EnemyBehaviourSO item in list) {
            _listOfCooldowns[item] = 0f;
        }
    }
    #endregion

    #region Cooldown
    /// <summary>
    /// Return the status of a skill cooldown
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool SkillInCooldown(EnemyBehaviourSO skill) {
        if (_listOfCooldowns.ContainsKey(skill)) {
            if (_listOfCooldowns[skill] > 0f) return true;
            else return false;
        }
        else return true;
    }

    /// <summary>
    /// Set the cooldown of a skill
    /// </summary>
    /// <param name="skill"></param>
    public void SetSkillCooldown(EnemyBehaviourSO skill) {
        if (!_listOfCooldowns.ContainsKey(skill)) return;

        _listOfCooldowns[skill] = skill.Cooldown;
        StartCoroutine(SkillCooldown(skill));
    }

    IEnumerator SkillCooldown(EnemyBehaviourSO skill) {

        while (_listOfCooldowns[skill] > 0) {
            _listOfCooldowns[skill] -= Time.deltaTime;
            yield return null;
        }

        _listOfCooldowns[skill] = 0f;
    }
    #endregion

    #endregion
}
