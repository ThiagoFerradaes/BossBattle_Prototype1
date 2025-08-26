using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageContext
{
    public float MinDamage;
    public float MaxDamage;
    public float Duration;
    public float Penetration;
    public bool HitShield;
    public DamageType TypeOfDamage;
    public Tags UnitToHitTag;
    public StatusManager StatusManager;

    public InstantDamageContext(float minDamage, float maxDamage, float hitBoxDuration, float penetration
        , bool hitShield, DamageType type, Tags tag, StatusManager status)
    {
        this.MinDamage = minDamage;
        this.MaxDamage = maxDamage;
        this.Duration = hitBoxDuration;
        this.Penetration = penetration;
        this.HitShield = hitShield;
        this.UnitToHitTag = tag;
        this.TypeOfDamage = type;
        this.StatusManager = status;
    }
}
public class InstantDamageHitBox : MonoBehaviour
{
    #region Parameters

    float _minDamage;
    float _maxDamage;
    float _duration;
    float _penetration;
    bool _hitShield;
    string _tag;
    StatusManager _statusManager;
    DamageType _damageType;

    #endregion

    #region Methods
    public void Initialize(InstantDamageContext context)
    {
        _minDamage = context.MinDamage;
        _maxDamage = context.MaxDamage;
        _duration = context.Duration;
        _penetration = context.Penetration;
        _hitShield = context.HitShield;
        _tag = context.UnitToHitTag.ToString();
        _statusManager = context.StatusManager;
        _damageType = context.TypeOfDamage;
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
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_tag)) return;

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

        if(_tag != Tags.Player.ToString())PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageType);
        health.TakeDamage(newDamage.Item1, _hitShield);
    }
    #endregion
}
