using System.Collections;
using UnityEngine;

public class LilianJudgmentDayManager : SkillObjectManager {
    // Components
    LilianJudgmentDaySO _info;
    ContinuosDamageHitBox _damageHitBox;

    Coroutine _durationRoutine, _damageRoutine;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) {
            _info = skill as LilianJudgmentDaySO;
        }

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.LooseAllEnergy();

        energyManager.SetCanGainEnergy(false);
    }

    public override void ThirdFunc() {
        base.ThirdFunc();

        healthManager.Heal(_info.InitialHeal);
    }
    public override void FourthFunc() {
        base.FourthFunc();

        // Desbloqueando inputs
        UnblockInputs();
    }

    IEnumerator Duration() {
        while (healthManager.ReturnCurrentHealth() > _info.HealthLimit) {
            yield return null;
        }

        _damageHitBox.End();
        _damageHitBox = null;

        _durationRoutine = null;
        if (_damageRoutine != null) {
            StopCoroutine(_damageRoutine);
            _damageRoutine = null;
        }

        energyManager.SetCanGainEnergy(true);

        End();
    }

    IEnumerator DamageToLilianRoutine() {
        while (true) {
            yield return new WaitForSeconds(_info.Atributes.DamageCooldown);

            float damageToLilian = _info.DamageToLilian;
            float currentHealth = healthManager.ReturnCurrentHealth();

            if (currentHealth - damageToLilian <= _info.HealthLimit) {
                float damage = currentHealth - _info.HealthLimit;
                healthManager.TakeDamage(damage);
            }
            else healthManager.TakeDamage(damageToLilian);
        }
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.SetParent(parent.transform);
        hitbox.transform.localScale = _info.Atributes.Size;
        hitbox.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DamageContext context = new(_info.Atributes, statusManager);

        ContinuosDamageHitBox damageHitbox = hitbox.GetComponent<ContinuosDamageHitBox>();
        _damageHitBox = damageHitbox;
        _damageHitBox.Initialize(context);

        _durationRoutine ??= StartCoroutine(Duration());
        _damageRoutine ??= StartCoroutine(DamageToLilianRoutine());
    }
}
