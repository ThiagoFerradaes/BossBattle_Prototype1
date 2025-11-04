using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class DamageContext
{
    public DamageAtributes Atributes;
    public StatusManager StatusManager;

    public DamageContext(DamageAtributes atributes, StatusManager status)
    {
        this.Atributes = atributes;
        this.StatusManager = status;
    }
}
public class InstantDamageHitBox : MonoBehaviour
{
    #region Parameters

    DamageAtributes _damageAtributes;
    StatusManager _statusManager;

    public event Action OnHit;
    bool _hasHitted;

    #endregion

    #region Methods
    public void Initialize(DamageContext context)
    {
        _damageAtributes = context.Atributes;
        _statusManager = context.StatusManager;

        gameObject.SetActive(true);
        StartCoroutine(AttackDuration());
    }
    IEnumerator AttackDuration()
    {
        float timer = 0;
        while (timer < _damageAtributes.HitBoxDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_damageAtributes.UnitsToHit.Any(tag => other.CompareTag(tag.ToString()))) return;

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


        (float, bool) newDamage;
        if (!_damageAtributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.CritRate) ||
            !_damageAtributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.CritDamage))
        {
            newDamage = DamageCalculator.CalculateDamage(
                _damageAtributes,
                _statusManager,
                recieverStatus
                );
        }
        else
        {
            newDamage = DamageCalculator.CalculateDamage(
                _damageAtributes,
                _statusManager,
                recieverStatus
                );
        }
        if (!other.CompareTag(Tags.Player.ToString())) PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageAtributes.DamageType);

        if (_damageAtributes.BreakShield) health.BreakShield();

        health.TakeDamage(newDamage.Item1, _damageAtributes.HitShield);

        if (!_hasHitted)
        {
            _hasHitted = true;
            OnHit?.Invoke();
        }
    }

    void End()
    {
        _hasHitted = false;
        OnHit = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }
    #endregion
}
