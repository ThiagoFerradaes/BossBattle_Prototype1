using System.Collections;
using UnityEngine;

public class GraciaBlueDashUltimateManager : MonoBehaviour {
    InstantDamageHitBox _hitbox;
    GraciaDashUltimateSO _info;
    Transform _parent;
    Coroutine _moveRoutine;
    StatusManager _status;
    public void Initialize(GraciaDashUltimateSO info, Transform parent, StatusManager statusManager) {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (_hitbox == null) _hitbox = GetComponent<InstantDamageHitBox>();
        if (_info == null) _info = info;
        if (_parent == null) _parent = parent;
        if (_status == null) _status = statusManager;

        _moveRoutine ??= StartCoroutine(WaitToMove());

    }

    IEnumerator WaitToMove() {
        yield return new WaitForSeconds(_info.BlueCooldownToMove);

        DamageContext newContext = new(_info.BlueAtributes, _status);
        _hitbox.Initialize(newContext);

        float timer = 0f;
        float distanceToTarget = Vector3.Distance(transform.position, _parent.position);

        while (distanceToTarget > _info.BlueDistanceLimitToPlayer) {

            // Calculo da direção 
            Vector3 direction = _parent.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);

            // Calculo da velocidade
            distanceToTarget = Vector3.Distance(transform.position, _parent.position);
            timer += Time.deltaTime;
            float speed = distanceToTarget / (_info.BlueShadowDurationToReachPlayer - timer);

            Vector3 nextPos = transform.position + speed * Time.deltaTime * transform.forward;

            float newDist = Vector3.Distance(nextPos, _parent.position);

            if (newDist > distanceToTarget) {
                transform.position = _parent.position;
                break;
            }

            transform.position = nextPos;

            yield return null;
        }

        _hitbox.ForceEnd();

        _moveRoutine = null;
    }

}
