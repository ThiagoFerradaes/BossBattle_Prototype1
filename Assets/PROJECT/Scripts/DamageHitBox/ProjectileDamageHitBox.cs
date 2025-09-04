using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileDamageHitBox : MonoBehaviour {

    float _minDamage;
    float _maxDamage;
    float _distance;
    float _speed;
    float _penetration;
    bool _hitShield;
    bool _breakShield;
    List<Tags> _tag = new();
    StatusManager _statusManager;
    DamageType _damageType;
    Dictionary<ExtraDamageContextAtributes, object> _extra = new();

    Coroutine _moveRoutine;

    public event Action OnHit;

    public void Initialize(DamageContext context) {
        _minDamage = context.MinDamage;
        _maxDamage = context.MaxDamage;
        _hitShield = context.HitShield;
        _statusManager = context.StatusManager;
        _damageType = context.TypeOfDamage;
        _tag = new(context.UnitsToHitTag);
        _extra = context.DictionaryOfExtraAtributes ?? new();

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Penetration, out var pen)) {
            _penetration = (float)pen;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Speed, out var speed)) {
            _speed = (float)speed;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Distance, out var distance)) {
            _distance = (float)distance;
        }

        if (_extra.TryGetValue(ExtraDamageContextAtributes.BreakShield, out var breakS)){
            _breakShield = (bool)breakS;
        }

        gameObject.SetActive(true);

        _moveRoutine ??= StartCoroutine(ProjectileMoveRoutine());
    }

    IEnumerator ProjectileMoveRoutine() {
        float duration = _distance/_speed;
        float timer = 0;

        while (timer < duration) {
            transform.position += _speed * Time.deltaTime * Vector3.forward;
            yield return null;
        }

        _moveRoutine = null;

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!_tag.Any(tag => other.CompareTag(tag.ToString()))) return;


        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!other.TryGetComponent<StatusManager>(out StatusManager recieverStatus)) return;

        if (!health.ReturnIfCanTakeDamage()) return;

        float damage = Random.Range(_minDamage, _maxDamage);

        (float, bool) newDamage = DamageCalculator.CalculateDamage(
            _damageType,
            damage,
            _penetration,
            _statusManager,
            recieverStatus
            );

        if (!other.CompareTag(Tags.Player.ToString())) PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageType);

        if (_breakShield) health.BreakShield();

        health.TakeDamage(newDamage.Item1, _hitShield);

        OnHit?.Invoke();

        End();
    }

    void End() {

        StopCoroutine(_moveRoutine);

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

}
