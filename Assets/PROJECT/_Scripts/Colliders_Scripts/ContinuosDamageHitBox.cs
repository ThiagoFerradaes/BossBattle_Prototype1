using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


public class ContinuosDamageHitBox : MonoBehaviour {
    // Atributos
    DamageAtributes _damageAtributes;
    StatusManager _dealerStatus;

    // Listas
    HashSet<GameObject> _listOfHealths = new();

    // Corrotinas
    Coroutine _durationCoroutine, _attackCooldownCoroutine;

    // Event
    public event Action OnHit, OnEnd;
    public event Action OnEnter, OnExit;

    public void Initialize(DamageContext context) {
        _damageAtributes = context.Atributes;
        _dealerStatus = context.StatusManager;

        gameObject.SetActive(true);
        _durationCoroutine ??= StartCoroutine(AttackDuration());
        _attackCooldownCoroutine ??= StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackDuration() {
        float timer = 0;
        while (timer < _damageAtributes.HitBoxDuration) {
            timer += Time.deltaTime;
            yield return null;
        }

        End();
    }

    IEnumerator AttackCooldown() {
        while (true) {
            List<GameObject> desactiveUnits = new();

            foreach (GameObject unit in _listOfHealths) {

                if (!unit.activeInHierarchy) {
                    desactiveUnits.Add(unit);
                    continue;
                }
                if (!unit.TryGetComponent<HealthManager>(out HealthManager health)) {
                    health = unit.GetComponentInParent<HealthManager>();
                    if (health == null) {
                        Debug.Log("No HealthManager found in this object or its parents");
                        continue;
                    }
                }
                if (!unit.TryGetComponent<StatusManager>(out StatusManager recieverManager)) {
                    recieverManager = unit.GetComponentInParent<StatusManager>();
                    if (recieverManager == null) {
                        Debug.Log("No StatusManager found in this object or its parents");
                        continue;
                    }
                }

                if (!health.ReturnIfCanTakeDamage()) continue;

                float newDamage = DamageCalculator.CalculateDamage(
                    _damageAtributes,
                    _dealerStatus,
                    recieverManager);
 

                if (unit.layer == LayerMask.NameToLayer("Enemy")) PopUpManager.Instance.DamageDone(
                    (int)newDamage, health.transform.position);
                health.TakeDamage(newDamage, _damageAtributes.HitShield);

                OnHit?.Invoke();

            }

            foreach (var enemy in desactiveUnits) {
                _listOfHealths.Remove(enemy);
            }

            yield return new WaitForSeconds(_damageAtributes.DamageCooldown);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!_damageAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        _listOfHealths.Add(other.gameObject);
        OnEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other) {
        if (!_damageAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        _listOfHealths.Remove(other.gameObject);
        OnExit?.Invoke();
    }

    public void End() {
        OnHit = null;
        OnEnter = null;
        OnExit = null;

        if (_durationCoroutine != null) {
            StopCoroutine(_durationCoroutine);
            _durationCoroutine = null;
        }

        if (_attackCooldownCoroutine != null) {
            StopCoroutine(_attackCooldownCoroutine);
            _attackCooldownCoroutine = null;
        }

        _listOfHealths.Clear();

        OnEnd?.Invoke();

        OnEnd = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    public void ChangeAtributes(DamageAtributes newAtributes) => _damageAtributes = newAtributes;
}
