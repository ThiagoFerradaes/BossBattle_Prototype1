using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



public class InstantDamageHitBox : MonoBehaviour {
    #region Parameters

    DamageAtributes _damageAtributes;
    StatusManager _statusManager;

    public event Action OnHit;
    bool _hasHitted;

    #endregion

    #region Methods
    public void Initialize(DamageContext context, bool hasTimer = true) {
        _damageAtributes = context.Atributes;
        _statusManager = context.StatusManager;

        gameObject.SetActive(true);
        if (hasTimer) StartCoroutine(AttackDuration());
    }
    public void ForceEnd() => End();
    IEnumerator AttackDuration() {
        float timer = 0;
        while (timer < _damageAtributes.HitBoxDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        End();
    }

    private void OnTriggerEnter(Collider other) {
        if (!_damageAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) {
            health = other.GetComponentInParent<HealthManager>();
            if (health == null) {
                Debug.Log("No HealthManager found in this object or its parents");
                return;
            }
        }
        if (!other.TryGetComponent<StatusManager>(out StatusManager recieverStatus)) {
            recieverStatus = other.GetComponentInParent<StatusManager>();
            if (recieverStatus == null) {
                Debug.Log("No StatusManager found in this object or its parents");
                return;
            }
        }

        if (!health.ReturnIfCanTakeDamage()) return;


        (float, bool) newDamage = DamageCalculator.CalculateDamage(
                _damageAtributes,
                _statusManager,
                recieverStatus
                );

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) PopUpManager.Instance.
                DamageDone((int)newDamage.Item1, other.transform.position, newDamage.Item2, _damageAtributes.DamageType);

        if (_damageAtributes.BreakShield) health.BreakShield();

        health.TakeDamage(newDamage.Item1, _damageAtributes.HitShield);

        if (!_hasHitted) {
            _hasHitted = true;
            OnHit?.Invoke();
        }
    }

    void End() {
        _hasHitted = false;
        OnHit = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }
    #endregion
}
