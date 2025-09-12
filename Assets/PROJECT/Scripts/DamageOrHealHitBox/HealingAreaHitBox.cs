using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealingAreaHitBox : MonoBehaviour {

    List<Tags> _listOfTags;
    HashSet<HealthManager> _listOfUnitsToHeal = new();

    Coroutine _durationRoutine, _healingRoutine;
    public void Initialize(float healing, float duration, float healingCooldown, List<Tags> listOfTags) {
        gameObject.SetActive(true);

        _listOfTags = listOfTags;

        _durationRoutine ??= StartCoroutine(Duration(duration));
        _healingRoutine ??= StartCoroutine(Healing(healing, healingCooldown));

    }

    IEnumerator Duration(float duration) {

        yield return new WaitForSeconds(duration);

        _listOfUnitsToHeal.Clear();

        _durationRoutine = null;
        StopCoroutine(_healingRoutine);
        _healingRoutine = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    IEnumerator Healing(float healAmount, float healCooldown) {
        while (true) {
            foreach (var unit in _listOfUnitsToHeal) {
                unit.Heal(healAmount);
            }
            yield return new WaitForSeconds(healCooldown);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!_listOfTags.Any(tag => other.CompareTag(tag.ToString()))) return;

        if (!other.TryGetComponent(out HealthManager health)) return;

        _listOfUnitsToHeal.Add(health);
    }

    private void OnTriggerExit(Collider other) {
        if (!_listOfTags.Any(tag => other.CompareTag(tag.ToString()))) return;

        if (!other.TryGetComponent(out HealthManager health)) return;

        _listOfUnitsToHeal.Remove(health);
    }
}
