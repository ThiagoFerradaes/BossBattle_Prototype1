using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LilianWingsOfHorrorObject : MonoBehaviour
{

    LilianWingsOfHorrorSO _info;
    StatusManager _status;
    Animator _anim;

    Coroutine _durationRoutine, _attackRountine;
    public void Initialize(StatusManager status, LilianWingsOfHorrorSO info) {
        _info = info;
        _status = status;

        if (_anim == null) _anim = GetComponentInChildren<Animator>();

        gameObject.SetActive(true);

        _durationRoutine ??= StartCoroutine(Duration());
        _attackRountine ??= StartCoroutine(AttackRoutine());
    }


    IEnumerator Duration() {
        yield return new WaitForSeconds(_info.SkillDamageAtributes.HitBoxDuration);

        if (_attackRountine != null) {
            StopCoroutine(_attackRountine);
            _attackRountine = null;
        }

        _durationRoutine = null;

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    IEnumerator AttackRoutine() {
        while (true) {
            yield return new WaitForSeconds(_info.SkillDamageAtributes.DamageCooldown);
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, _info.RadiusOfAttack, _info.LayersToHit);

            if (enemiesInRange.Length <= 0) continue;

            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < enemiesInRange.Length; i++) {
                float distance = Vector3.Distance(enemiesInRange[i].transform.position, transform.position);

                if (distance < closestDistance) {
                    closestEnemy = enemiesInRange[i].transform;
                    closestDistance = distance;
                }
            }

            if (closestEnemy == null) continue;

            Vector3 dir = (closestEnemy.position - transform.position).normalized;
            Quaternion skullDir = Quaternion.LookRotation(dir);

            float angle = Quaternion.Angle(transform.rotation, skullDir);
            float rotationDuration = angle / _info.RotationSpeed;

            yield return transform.DORotate(skullDir.eulerAngles, rotationDuration).WaitForCompletion();

            yield return StartCoroutine(Attack());
        }
    }

    IEnumerator Attack() {
        SkillAnimationEvent skillEvent = _info.Prefabs[1][0];

        _anim.SetTrigger(_info.WingsOfHorrorAnimationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsName(_info.WingsOfHorrorAnimationName) && stateInfo.normalizedTime < skillEvent.TimeToSpawnPreFab) {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        }

        int hashState = stateInfo.GetHashCode();

        GameObject projectile = PoolingManager.Instance.ReturnPrefabFromPool(skillEvent.PreFab, TypeOfSkillPrefab.Hitbox);
        projectile.transform.SetPositionAndRotation(skillEvent.PreFabPosition, transform.rotation);
        DamageContext newContext = new(_info.SkillDamageAtributes, _status);

        projectile.GetComponent<ProjectileDamageHitBox>().Initialize(newContext);

        while (stateInfo.GetHashCode() == hashState) yield return null; 
    }
    private void OnDestroy() {
        transform.DOKill();
    }
}
