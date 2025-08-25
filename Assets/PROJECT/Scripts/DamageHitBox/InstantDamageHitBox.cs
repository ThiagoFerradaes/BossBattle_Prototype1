using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageContext
{
    public float Damage;
    public float Duration;
    public float Penetration;
    public bool HitShield;
    public DamageType TypeOfDamage;
    public Tags UnitToHitTag;
    public StatusManager StatusManager;

    public InstantDamageContext(float damage, float hitBoxDuration, float penetration
        , bool hitShield, DamageType type, Tags tag, StatusManager status)
    {
        this.Damage = damage;
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

    float _damage;
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
        _damage = context.Damage;
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
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_tag)) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;
        if (!other.TryGetComponent<StatusManager>(out StatusManager recieverStatus)) return;

        if (!health.ReturnIfCanTakeDamage()) return;

        (float, bool) damage = DamageCalculator.CalculateDamage(
            _damageType,
            _damage,
            _penetration,
            _statusManager,
            recieverStatus
            );

        if(_tag == Tags.Enemy.ToString())PopUpManager.Instance.DamageDone((int)damage.Item1, other.transform.position, damage.Item2);
        health.TakeDamage(damage.Item1, _hitShield);
    }
    #endregion
}
