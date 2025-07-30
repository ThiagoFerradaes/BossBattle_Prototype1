using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenManager : MonoBehaviour
{
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] List<EnemySkillSO> _listOfSkillss = new();
    [SerializeField] List<GameObject> _tentaclesList = new();

    EnemyCooldownManager _enemyCooldownManager;
    Transform _player;

    private void Awake() {
        _enemyCooldownManager = GetComponent<EnemyCooldownManager>();
    }

    private void Start() {
        StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        ChooseAnAttack();
    }
    void ChooseAnAttack() {
        int priority = -1;
        EnemySkillSO currentSkill = null;
        foreach (var skill in _listOfSkillss) {
            if (skill.Priority >= priority && !_enemyCooldownManager.SkillInCooldown(skill)) {
                currentSkill = skill;
                priority = skill.Priority;
            }
        }

        if(currentSkill != null) Attack(currentSkill);
    }

    void Attack(EnemySkillSO skill) {
        _enemyCooldownManager.SetSkillCooldown(skill);

        if (skill is KrakenRandomAttack) {

        }
        else if (skill is KrakenHalfAttack) {

        }
        else if (skill is KrakenSpinningAttack) {

        }
        else if (skill is KrakenCrossAttack) {

        }
        else if (skill is KrakenRandomAttack) {

        }
    }


}
