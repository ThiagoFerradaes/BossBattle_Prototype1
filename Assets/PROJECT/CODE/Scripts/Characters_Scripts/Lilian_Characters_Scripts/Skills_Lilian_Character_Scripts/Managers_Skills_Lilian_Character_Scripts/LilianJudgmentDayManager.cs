using System.Collections;
using UnityEngine;

public class LilianJudgmentDayManager : SkillObjectManager {
    // Components
    LilianJudgmentDaySO _info;
    EnergyManager _energyManager;
    HealthManager _healthManager;
    ContinuosDamageHitBox _damageHitBox;

    Coroutine _durationRoutine, _damageRoutine;
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) {
            _info = skill as LilianJudgmentDaySO;
            _energyManager = parent.GetComponent<EnergyManager>();
            _healthManager = parent.GetComponent<HealthManager>();
        }

        gameObject.SetActive(true);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        base.FirstFunc();

        _energyManager.LooseAllEnergy();

        _energyManager.SetCanGainEnergy(false);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        // Desbloqueando inputs
        UnblockInputs();
    }

    IEnumerator Duration() {
        while (_healthManager.ReturnCurrentHealth() > 1) {
            yield return null;
        }

        _damageHitBox.End();
        _damageHitBox = null;

        _durationRoutine = null;
        if (_damageRoutine != null) {
            StopCoroutine(_damageRoutine);
            _damageRoutine = null;
        }
        End();
    }

    IEnumerator DamageToLilianRoutine() {
        while (true) {
            yield return new WaitForSeconds(_info.Atributes.DamageCooldown);
            _healthManager.TakeDamage(_info.DamageToLilian);
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
