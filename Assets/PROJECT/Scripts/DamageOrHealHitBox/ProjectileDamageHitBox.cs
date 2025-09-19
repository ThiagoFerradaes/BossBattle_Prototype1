using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileDamageHitBox : MonoBehaviour
{

    float _minDamage;
    float _maxDamage;
    float _distance;
    float _speed;
    float _penetration;
    float _critChance;
    float _critDamage;
    bool _hitShield;
    bool _breakShield;
    bool _crossEnemy;
    List<Tags> _tag = new();
    StatusManager _statusManager;
    DamageType _damageType;
    Dictionary<ExtraDamageContextAtributes, object> _extra = new();

    Coroutine _moveRoutine;

    public event Action OnHit;

    public void Initialize(DamageContext context)
    {
        _minDamage = context.MinDamage;
        _maxDamage = context.MaxDamage;
        _hitShield = context.HitShield;
        _statusManager = context.StatusManager;
        _damageType = context.TypeOfDamage;
        _tag = new(context.UnitsToHitTag);
        _extra = context.DictionaryOfExtraAtributes ?? new();

        ResetVariables();

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Penetration, out var pen))
        {
            _penetration = (float)pen;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Speed, out var speed))
        {
            _speed = (float)speed;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Distance, out var distance))
        {
            _distance = (float)distance;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.BreakShield, out var breakS))
        {
            _breakShield = (bool)breakS;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.CritRate, out var critChance))
        {
            _critChance = (float)critChance;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.CritDamage, out var critDamage))
        {
            _critDamage = (float)critDamage;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.CrossEnemy, out var crossEnemy))
        {
            _crossEnemy = (bool)crossEnemy;
        }

        gameObject.SetActive(true);

        _moveRoutine ??= StartCoroutine(ProjectileMoveRoutine());
    }

    void ResetVariables()
    {
        _penetration = 0;
        _speed = 0;
        _distance = 0;
        _critDamage = 0;
        _critChance = 0;
        _breakShield = false;
        _crossEnemy = false;
    }

    IEnumerator ProjectileMoveRoutine()
    {
        float duration = _distance / _speed;
        float timer = 0;

        while (timer < duration)
        {
            transform.position += _speed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_tag.Any(tag => other.CompareTag(tag.ToString()))) return;


        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!other.TryGetComponent<StatusManager>(out StatusManager recieverStatus)) return;

        if (!health.ReturnIfCanTakeDamage()) return;

        float damage = Random.Range(_minDamage, _maxDamage);

        (float, bool) newDamage = DamageCalculator.CalculateDamage(
            _damageType,
            damage,
            _penetration,
            _critChance,
            _critDamage,
            _statusManager,
            recieverStatus
            );

        if (!other.CompareTag(Tags.Player.ToString())) PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageType);

        if (_breakShield) health.BreakShield();

        health.TakeDamage(newDamage.Item1, _hitShield);

        OnHit?.Invoke();

        if (!_crossEnemy) End();
    }

    void End()
    {
        OnHit = null;

        StopCoroutine(_moveRoutine);

        _moveRoutine = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

}
