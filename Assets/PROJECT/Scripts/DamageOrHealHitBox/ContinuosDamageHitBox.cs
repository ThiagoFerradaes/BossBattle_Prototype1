using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


public class ContinuosDamageHitBox : MonoBehaviour
{
    // Atributos
    float _minDamagePerTick;
    float _maxDamagePerTick;
    float _damageCooldown;
    float _duration;
    float _penetrarion;
    bool _hitShield;
    DamageType _type;
    List<Tags> _typeOfUnit = new();
    StatusManager _dealerStatus;

    // Listas
    Dictionary<ExtraDamageContextAtributes, object> _extra = new();
    HashSet<GameObject> _listOfHealths = new();

    // Corrotinas
    Coroutine _durationCoroutine, _attackCooldownCoroutine;

    // Event
    public event Action OnHit;

    public void Initialize(DamageContext context)
    {
        _minDamagePerTick = context.MinDamage;
        _maxDamagePerTick = context.MaxDamage;
        _duration = context.Duration;
        _type = context.TypeOfDamage;
        _dealerStatus = context.StatusManager;
        _hitShield = context.HitShield;
        _typeOfUnit = new(context.UnitsToHitTag);
        _extra = context.DictionaryOfExtraAtributes ?? new();

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Penetration, out var pen))
        {
            _penetrarion = (float)pen;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.DamageCooldown, out var damageCooldown))
        {
            _damageCooldown = (float)damageCooldown;
        }

        gameObject.SetActive(true);
        _durationCoroutine ??= StartCoroutine(AttackDuration());
        _attackCooldownCoroutine ??= StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackDuration()
    {
        float timer = 0;
        while (timer < _duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        End();
    }

    IEnumerator AttackCooldown()
    {
        while (true)
        {
            List<GameObject> desactiveUnits = new();

            foreach (GameObject unit in _listOfHealths)
            {

                if (!unit.activeInHierarchy)
                {
                    desactiveUnits.Add(unit);
                    continue;
                }

                if (unit.TryGetComponent<HealthManager>(out HealthManager health) &&
                    unit.TryGetComponent<StatusManager>(out StatusManager recieverManager))
                {

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

                    OnHit?.Invoke();
                }
            }

            foreach (var enemy in desactiveUnits)
            {
                _listOfHealths.Remove(enemy);
            }

            yield return new WaitForSeconds(_damageCooldown);
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

    public void End()
    {
        OnHit = null;

        _durationCoroutine = null;
        _attackCooldownCoroutine = null;

        _listOfHealths.Clear();
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }
}
