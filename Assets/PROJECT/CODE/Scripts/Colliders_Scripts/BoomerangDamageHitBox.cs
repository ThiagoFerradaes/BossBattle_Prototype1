using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BoomerangDamageHitBox : MonoBehaviour
{
    DamageAtributes _damageAtributes;
    StatusManager _statusManager;

    Coroutine _moveRoutine;

    public event Action<GameObject> OnHit;

    public void Initialize(DamageContext context) {
        _damageAtributes = context.Atributes;
        _statusManager = context.StatusManager;

        gameObject.SetActive(true);

        _moveRoutine ??= StartCoroutine(BoomerangMoveRoutine());
    }

    IEnumerator BoomerangMoveRoutine() {
        float duration = _damageAtributes.Distance / _damageAtributes.Speed;
        float timer = 0f;

        while (timer < duration) {
            transform.position += _damageAtributes.Speed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_damageAtributes.TimeStopped);

        timer = 0f;
        float distanceToTarget = Vector3.Distance(transform.position, _statusManager.transform.position);
        while (distanceToTarget > _damageAtributes.MinDistanceBack) {

            // Calculo da direção 
            Vector3 direction = _statusManager.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);

            // Calculo da velocidade
            distanceToTarget = Vector3.Distance(transform.position, _statusManager.transform.position);
            timer += Time.deltaTime;
            float speed = distanceToTarget / (duration - timer);

            Vector3 nextPos = transform.position + speed * Time.deltaTime * transform.forward;

            float newDist = Vector3.Distance(nextPos, _statusManager.transform.position);

            if (newDist > distanceToTarget) {
                transform.position = _statusManager.transform.position;
                End();
                yield break;
            }

            transform.position = nextPos;

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

        OnHit?.Invoke(other.gameObject);
    }

    void End() {
        OnHit = null;

        StopCoroutine(_moveRoutine);

        _moveRoutine = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }
}
