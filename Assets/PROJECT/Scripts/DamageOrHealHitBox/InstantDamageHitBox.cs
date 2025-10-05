using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ExtraDamageContextAtributes
{
    Penetration,
    BreakShield,

    // Projectile
    Distance,
    Speed,
    CrossEnemy,

    // Dps
    DamageCooldown,

    // Crit
    CritRate,
    CritDamage

}
public class DamageContext
{
    public float MinDamage;
    public float MaxDamage;
    public float Duration;
    public bool HitShield;
    public DamageType TypeOfDamage;
    public List<Tags> UnitsToHitTag;
    public StatusManager StatusManager;

    public Dictionary<ExtraDamageContextAtributes, object> DictionaryOfExtraAtributes;

    public DamageContext(float minDamage, float maxDamage, float hitBoxDuration, bool hitShield,
        DamageType type, List<Tags> tags, StatusManager status, Dictionary<ExtraDamageContextAtributes, object> extraAtributes = null)
    {
        this.MinDamage = minDamage;
        this.MaxDamage = maxDamage;
        this.Duration = hitBoxDuration;
        this.HitShield = hitShield;
        this.UnitsToHitTag = tags;
        this.TypeOfDamage = type;
        this.StatusManager = status;
        this.DictionaryOfExtraAtributes = extraAtributes ?? new();
    }
}
public class InstantDamageHitBox : MonoBehaviour
{
    #region Parameters

    float _minDamage;
    float _maxDamage;
    float _duration;
    float _penetration;
    float _critRate;
    float _critDamage;
    bool _hitShield;
    bool _breakShield;
    List<Tags> _tag = new();
    StatusManager _statusManager;
    DamageType _damageType;
    Dictionary<ExtraDamageContextAtributes, object> _extra = new();

    public event Action OnHit;
    bool _hasHitted;

    #endregion

    #region Methods
    public void Initialize(DamageContext context)
    {
        _minDamage = context.MinDamage;
        _maxDamage = context.MaxDamage;
        _duration = context.Duration;
        _hitShield = context.HitShield;
        _statusManager = context.StatusManager;
        _damageType = context.TypeOfDamage;
        _tag = new(context.UnitsToHitTag);

        _extra = context.DictionaryOfExtraAtributes ?? new();

        if (_extra.TryGetValue(ExtraDamageContextAtributes.Penetration, out var pen))
            _penetration = (float)pen;

        if (_extra.TryGetValue(ExtraDamageContextAtributes.BreakShield, out var breakS))
            _breakShield = (bool)breakS;

        if (_extra.TryGetValue(ExtraDamageContextAtributes.CritRate, out var critRate))
            _critRate = (float)critRate;

        if (_extra.TryGetValue(ExtraDamageContextAtributes.CritDamage, out var critDamage))
            _critDamage = (float)critDamage;

        gameObject.SetActive(true);
        StartCoroutine(AttackDuration());
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

    private void OnTriggerEnter(Collider other)
    {
        if (!_tag.Any(tag => other.CompareTag(tag.ToString()))) return;


        if (!other.TryGetComponent<HealthManager>(out HealthManager health))
        {
            health = other.GetComponentInParent<HealthManager>();
            if (health == null)
            {
                Debug.Log("No HealthManager found in this object or its parents");
                return;
            }
        }
        if (!other.TryGetComponent<StatusManager>(out StatusManager recieverStatus))
        {
            recieverStatus = other.GetComponentInParent<StatusManager>();
            if (recieverStatus == null)
            {
                Debug.Log("No StatusManager found in this object or its parents");
                return;
            }
        }

        if (!health.ReturnIfCanTakeDamage()) return;

        float damage = Random.Range(_minDamage, _maxDamage);

        (float, bool) newDamage;
        if (_critRate == 0 && _critDamage == 0)
        {
            newDamage = DamageCalculator.CalculateDamage(
                _damageType,
                damage,
                _penetration,
                _statusManager,
                recieverStatus
                );
        }
        else
        {
            newDamage = DamageCalculator.CalculateDamage(
                _damageType,
                damage,
                _penetration,
                _critRate,
                _critDamage,
                _statusManager,
                recieverStatus
                );
        }
        if (!other.CompareTag(Tags.Player.ToString())) PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageType);

        if (_breakShield) health.BreakShield();

        health.TakeDamage(newDamage.Item1, _hitShield);

        if (!_hasHitted)
        {
            _hasHitted = true;
            OnHit?.Invoke();
        }
    }

    void End()
    {
        _hasHitted = false;
        _breakShield = false;
        _critDamage = 0;
        _critRate = 0;
        OnHit = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }
    #endregion
}
