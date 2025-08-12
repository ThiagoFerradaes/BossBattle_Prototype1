using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuosDamageContext {
    public float Damage;
    public float Duration;
    public float Penetration;
    public float DamageCooldown;
    public bool HitShield;
    public DamageType TypeOfDamage;
    public Tags UnitToHitTag;
    public StatusManager StatusManager;
    public List<Modifiers> ListOfModifiers;

    public ContinuosDamageContext(float damage, float hitBoxDuration, float penetration, float damageCooldown
        , bool hitShield, Tags tag, DamageType type, StatusManager status, List<Modifiers> listOfModifiers = null) {
        this.Damage = damage;
        this.Duration = hitBoxDuration;
        this.DamageCooldown = damageCooldown;
        this.Penetration = penetration;
        this.HitShield = hitShield;
        this.TypeOfDamage = type;
        this.UnitToHitTag = tag;
        this.StatusManager = status;
        this.ListOfModifiers = listOfModifiers ?? new List<Modifiers>();
    }
}

public class ContinuosDamageHitBox : MonoBehaviour
{
    float _damagePerTick;
    float _damageCooldown;
    float _duration;
    float _penetrarion;
    DamageType _type;
    string _typeOfUnit;
    bool _hitShield;
    StatusManager _dealerStatus;

    HashSet<GameObject> _listOfHealths = new();
    public void Initialize(ContinuosDamageContext context)
    {
        _damagePerTick = context.Damage;
        _duration = context.Duration;
        _damageCooldown = context.DamageCooldown;
        _penetrarion = context.Penetration;
        _type = context.TypeOfDamage;
        _dealerStatus = context.StatusManager;
        _hitShield = context.HitShield;
        _typeOfUnit = context.UnitToHitTag.ToString();

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
                if (unit.TryGetComponent<HealthManager>(out HealthManager health) && 
                    unit.TryGetComponent<StatusManager>(out StatusManager recieverManager)) {

                    float damage = DamageCalculator.CalculateDamage(
                        _type,
                        _damagePerTick,
                        _penetrarion,
                        _dealerStatus,
                        recieverManager
                        );


                    health.TakeDamage(damage, _hitShield);
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
