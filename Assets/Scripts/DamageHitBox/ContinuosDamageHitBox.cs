using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuosDamageHitBox : MonoBehaviour
{
    float _damagePerTick;
    float _damageCooldown;
    float _duration;
    string _typeOfUnit;

    HashSet<GameObject> _listOfHealths = new();
    public void Initialize(float damage, float duration, string typeOfUnitToDamage, float damageCooldown)
    {
        _damagePerTick = damage; _duration = duration; _damageCooldown = damageCooldown; _typeOfUnit = typeOfUnitToDamage;
        gameObject.SetActive(true);
        StartCoroutine(AttackDuration());
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackDuration()
    {
        float timer = 0;
        while (timer < _duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _listOfHealths.Clear();
        gameObject.SetActive(false);
    }

    IEnumerator AttackCooldown()
    {
        while (true)
        {
            // Dar dano
            //foreach (var unit in _listOfHealths)
            //{}
            yield return new WaitForSeconds (_damageCooldown);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_typeOfUnit)) return;

        _listOfHealths.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_typeOfUnit)) return;

        _listOfHealths.Remove(other.gameObject);
    }
}
