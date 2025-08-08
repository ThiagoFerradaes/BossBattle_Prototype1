using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuosDamageContext {
    public float Damage;
    public float Duration;
    public float DamageCooldown;
    public bool IsTrueDamage;
    public Tags UnitToDamageTag;
    public List<Modifiers> ListOfModifiers;

    public ContinuosDamageContext(float damage, float hitBoxDuration, float damageCooldown
        , bool isTrueDamage, Tags tag, List<Modifiers> listOfModifiers = null) {
        this.Damage = damage;
        this.Duration = hitBoxDuration;
        this.DamageCooldown = damageCooldown;
        this.IsTrueDamage = isTrueDamage;
        this.UnitToDamageTag = tag;
        this.ListOfModifiers = listOfModifiers ?? new List<Modifiers>();
    }
}

public class ContinuosDamageHitBox : MonoBehaviour
{
    float _damagePerTick;
    float _damageCooldown;
    float _duration;
    string _typeOfUnit;
    bool _isTrueDamage;

    HashSet<GameObject> _listOfHealths = new();
    public void Initialize(ContinuosDamageContext context)
    {
        _damagePerTick = context.Damage; _duration = context.Duration;
        _damageCooldown = context.DamageCooldown; _typeOfUnit = context.UnitToDamageTag.ToString();

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
            foreach (GameObject unit in _listOfHealths) {
                if (unit.TryGetComponent<HealthManager>(out HealthManager health)) {
                    health.TakeDamage(_damagePerTick, _isTrueDamage);
                }
            }
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
