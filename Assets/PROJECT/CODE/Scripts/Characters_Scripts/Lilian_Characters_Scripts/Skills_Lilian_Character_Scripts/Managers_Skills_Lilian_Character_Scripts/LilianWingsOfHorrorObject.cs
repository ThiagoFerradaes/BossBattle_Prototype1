using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LilianWingsOfHorrorObject : MonoBehaviour {

    LilianWingsOfHorrorSO _info;
    StatusManager _status;
    HealthManager _health;
    EnergyManager _energy;
    Animator _anim;

    Coroutine _durationRoutine, _attackRountine;
    public void Initialize(StatusManager status, LilianWingsOfHorrorSO info, HealthManager health, EnergyManager energy) {
        _info = info;
        _status = status;
        _health = health;
        _energy = energy;

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
            Collider[] enemiesInRange = new Collider[100];
            int amountOfEnemies = Physics.OverlapSphereNonAlloc(transform.position, _info.RadiusOfAttack, enemiesInRange, _info.LayersToHit);

            if (amountOfEnemies <= 0) continue;

            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < amountOfEnemies; i++) {
                HealthManager enemyHealth = null;

                if (enemiesInRange[i].TryGetComponent<HealthManager>(out HealthManager health)) {
                    enemyHealth = health;
                }
                else if (enemiesInRange[i].GetComponentInChildren<HealthManager>() != null) {
                    enemyHealth = enemiesInRange[i].GetComponentInChildren<HealthManager>();
                }
                else if (enemiesInRange[i].GetComponentInParent<HealthManager>() != null) {
                    enemyHealth = enemiesInRange[i].GetComponentInParent<HealthManager>();
                }

                if (enemyHealth == null || enemyHealth.ReturnIfIsDead() || !enemyHealth.ReturnIfCanTakeDamage()) continue;

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

            yield return transform.DOLookAt(closestEnemy.position, rotationDuration).WaitForCompletion();

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

        int attackStateHash = stateInfo.fullPathHash;

        GameObject projectile = PoolingManager.Instance.ReturnPrefabFromPool(skillEvent.PreFab, TypeOfSkillPrefab.Hitbox);
        projectile.transform.SetPositionAndRotation(transform.position + skillEvent.PreFabPosition, transform.rotation);
        DamageContext newContext = new(_info.SkillDamageAtributes, _status);

        ProjectileDamageHitBox collider = projectile.GetComponent<ProjectileDamageHitBox>();
        collider.Initialize(newContext);
        collider.OnHit += () => {
            _energy.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        _health.TakeDamage(_info.HealthPercentLostPerAttack / 100 * _health.ReturnCurrentHealth());

        do {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash);
    }
    private void OnDestroy() {
        transform.DOKill();
    }
}
