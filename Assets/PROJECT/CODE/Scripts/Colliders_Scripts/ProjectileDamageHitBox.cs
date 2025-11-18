using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileDamageHitBox : MonoBehaviour {

    DamageAtributes _damageAtributes;
    StatusManager _statusManager;

    Coroutine _moveRoutine;

    public event Action OnHit;
    public event Action<Collider> OnCollision;

    public void Initialize(DamageContext context) {
        _damageAtributes = context.Atributes;
        _statusManager = context.StatusManager;

        gameObject.SetActive(true);

        _moveRoutine ??= StartCoroutine(ProjectileMoveRoutine());
    }

    IEnumerator ProjectileMoveRoutine() {
        float duration = 
            _damageAtributes.Distance / _damageAtributes.Speed;
        float timer = 0;

        while (timer < duration) {
            transform.position += _damageAtributes.Speed * Time.deltaTime * transform.forward;
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

        OnHit?.Invoke();
        OnCollision?.Invoke(other);

        if (!_damageAtributes.CrossEnemy) End();
    }

    void End() {
        OnHit = null;
        OnCollision = null;

        StopCoroutine(_moveRoutine);

        _moveRoutine = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

}
