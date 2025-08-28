using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContinuosDamageContext {
    public float MinDamage;
    public float MaxDamage;
    public float Duration;
    public float Penetration;
    public float DamageCooldown;
    public bool HitShield;
    public DamageType TypeOfDamage;
    public List<Tags> UnitToHitTag;
    public StatusManager StatusManager;

    public ContinuosDamageContext(float minDamage, float maxDamage, float hitBoxDuration, float penetration, float damageCooldown
        , bool hitShield, List<Tags> tag, DamageType type, StatusManager status) {
        this.MinDamage = minDamage;
        this.MaxDamage = maxDamage;
        this.Duration = hitBoxDuration;
        this.DamageCooldown = damageCooldown;
        this.Penetration = penetration;
        this.HitShield = hitShield;
        this.TypeOfDamage = type;
        this.UnitToHitTag = tag;
        this.StatusManager = status;
    }
}

public class ContinuosDamageHitBox : MonoBehaviour
{
    float _minDamagePerTick;
    float _maxDamagePerTick;
    float _damageCooldown;
    float _duration;
    float _penetrarion;
    DamageType _type;
    List<Tags> _typeOfUnit = new();
    bool _hitShield;
    StatusManager _dealerStatus;

    HashSet<GameObject> _listOfHealths = new();
    public void Initialize(ContinuosDamageContext context)
    {
        _minDamagePerTick = context.MinDamage;
        _maxDamagePerTick = context.MaxDamage;
        _duration = context.Duration;
        _damageCooldown = context.DamageCooldown;
        _penetrarion = context.Penetration;
        _type = context.TypeOfDamage;
        _dealerStatus = context.StatusManager;
        _hitShield = context.HitShield;
        _typeOfUnit = new(context.UnitToHitTag);

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
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    IEnumerator AttackCooldown()
    {
        while (true)
        {
            List<GameObject> desactiveUnits = new();

            foreach (GameObject unit in _listOfHealths) {

                if (!unit.activeInHierarchy) {
                    desactiveUnits.Add(unit);
                    continue;
                }

                if (unit.TryGetComponent<HealthManager>(out HealthManager health) && 
                    unit.TryGetComponent<StatusManager>(out StatusManager recieverManager)) {

                    if (!health.ReturnIfCanTakeDamage()) continue;

                    float damage = UnityEngine.Random.Range(_minDamagePerTick, _maxDamagePerTick);

                    (float, bool) newDamage = DamageCalculator.CalculateDamage(
                        _type,
                        damage,
                        _penetrarion,
                        _dealerStatus,
                        recieverManager
                        );

                    if (unit.CompareTag(Tags.Enemy.ToString())) PopUpManager.Instance.DamageDone(
                        (int)newDamage.Item1, health.transform.position, newDamage.Item2, _type);
                    health.TakeDamage(newDamage.Item1, _hitShield);
                }
            }

            foreach(var enemy in desactiveUnits) {
                _listOfHealths.Remove(enemy);
            }

            yield return new WaitForSeconds (_damageCooldown);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_typeOfUnit.Any(tag => other.CompareTag(tag.ToString()))) return;

        _listOfHealths.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_typeOfUnit.Any(tag => other.CompareTag(tag.ToString()))) return;

        _listOfHealths.Remove(other.gameObject);
    }

}
