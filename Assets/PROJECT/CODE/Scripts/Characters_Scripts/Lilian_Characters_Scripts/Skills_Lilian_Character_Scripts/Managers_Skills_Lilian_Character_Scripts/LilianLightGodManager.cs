using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class LilianLightGodManager : SkillObjectManager {
    #region Variables
    LilianLightGodSO _info;

    [SerializeField] List<GameObject> listOfGodsObjects = new();
    int _godIndex;
    bool _isShooting;
    float _skillTimer;

    Coroutine _selfDamageRoutine, _skillDurationRoutine;
    Action<float> _onHeal;
    #endregion

    #region Initialize
    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (!gameObject.activeInHierarchy) {
            Initialize(skill);

            animationCoroutine ??= StartCoroutine(AttackCoroutine());
        }

        else {
            ShootBeam();
        }
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as LilianLightGodSO;

        transform.SetParent(parent.transform, false);
        transform.SetLocalPositionAndRotation(_info.ManagerLocalPosition, Quaternion.identity);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        _onHeal = (float value) => { TurnGodOn(); };
    }
    #endregion

    #region Override
    public override void FirstFunc() {
        base.FirstFunc();

        if (_isShooting) return;

        energyManager.SetCanGainEnergy(false);
    }

    public override void ThirdFunc() {
        if (_isShooting) return;

        healthManager.Heal(_info.HealthToHealBeforeUlt);

        TurnGodOn();

        _selfDamageRoutine ??= StartCoroutine(SelfDamageCooldownRoutine());
        _skillDurationRoutine ??= StartCoroutine(DurationTimer());

        healthManager.OnHeal += _onHeal;
    }

    public override void FourthFunc() {
        base.FourthFunc();

        if (!_isShooting) UnblockInputs();
        else {
            UnblockInputs();
            End();
        }
    }
    #endregion

    void TurnGodOn() {
        if (_godIndex > listOfGodsObjects.Count - 1) {
            return;
        }

        listOfGodsObjects[_godIndex].SetActive(true);

        _godIndex++;

        if (_godIndex > listOfGodsObjects.Count - 1) {
            ShootBeam();
        }

    }
    IEnumerator SelfDamageCooldownRoutine() {
        float currentHealthPercent = healthManager.ReturnCurrentHealth() / healthManager.ReturnMaxHealth();

        while (currentHealthPercent > _info.PercentOfMinHealth / 100) {
            healthManager.TakeDamage(_info.SelfDamageLostOverTime);
            currentHealthPercent = healthManager.ReturnCurrentHealth() / healthManager.ReturnMaxHealth();
            yield return new WaitForSeconds(_info.CooldownBetweenSelfDamage);
        }

        ShootBeam();
    }
    IEnumerator DurationTimer() {
        while (true) {
            _skillTimer += Time.deltaTime;
            yield return null;
        }
    }
    void ShootBeam() {
        if (_isShooting) return;

        _isShooting = true;

        energyManager.LooseAllEnergy();

        if (_selfDamageRoutine != null) {
            StopCoroutine(_selfDamageRoutine);
            _selfDamageRoutine = null;
        }

        if (_skillDurationRoutine != null) {
            StopCoroutine(_skillDurationRoutine);
            _skillDurationRoutine = null;
        }

        animationCoroutine ??= StartCoroutine(AttackCoroutine());

        movementManager.BlockWalk(true);
        skillManager.BlockAllSkills(true);
    }

    #region Instantiate
    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        if (!_isShooting) return;

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        // SIZE
        Vector3 size = DecideBeamLocalScale();
        preFab.transform.localScale = size;

        // POSITION
        preFab.transform.SetParent(parent.transform, false);
        Vector3 pos = new(prefabInfo.PreFabPosition.x, prefabInfo.PreFabPosition.y, size.z / 2);
        preFab.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        preFab.transform.SetParent(null);

        // ATRIBUTES

        DamageAtributes newAtribute = new(_info.Atributes) {
            Damage = DecideDamage(),
            DamageCooldown = DecideDamageCooldown()
        };

        DamageContext newContext = new(newAtribute, statusManager);

        ContinuosDamageHitBox hitbox = preFab.GetComponent<ContinuosDamageHitBox>();
        hitbox.Initialize(newContext);
    }
    Vector3 DecideBeamLocalScale() {
        Vector3 finalSize;

        finalSize.y = _info.Atributes.Size.y;
        finalSize.x = _info.Atributes.Size.x + (_info.BeamSizeMultiplierByAmountOfGods * _godIndex);
        finalSize.z = _info.Atributes.Size.z;

        return finalSize;
    }

    float DecideDamageCooldown() {
        return _info.Atributes.DamageCooldown - (_info.BeamDamageCooldownByAmountOfGods * _godIndex);
    }

    float DecideDamage() {
        return _info.Atributes.Damage + (_info.BeamDamageMultiplierByAmountOfGods * _skillTimer);
    }

    #endregion
    public override void End() {
        healthManager.OnHeal -= _onHeal;

        _godIndex = 0;

        _skillTimer = 0;

        _isShooting = false;

        if (_selfDamageRoutine != null) {
            StopCoroutine(_selfDamageRoutine);
            _selfDamageRoutine = null;
        }

        foreach (var god in listOfGodsObjects) {
            god.SetActive(false);
        }

        energyManager.SetCanGainEnergy(true);

        base.End();
    }
}
