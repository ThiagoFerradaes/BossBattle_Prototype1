using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeAttackManager : SkillObjectManager {

    #region Parameter
    // Components
    AxeSkillSO _info;
    WeaponManager _weaponManager;

    // Booleans
    bool _isHoldingInput;

    // Floats
    float _chargeTimer;

    // Coroutine
    Coroutine _chargeTimeCoroutine;
    Coroutine _attackCoroutine;

    // Events
    public static event Action OnWeaponChange;

    #endregion

    #region Methods
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

        Initialize(skill);

        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            _isHoldingInput = true;
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            _preCasted = false;
            _isHoldingInput = false;
            OnRelease(skill);
        }
    }

    void Initialize(SkillSO skill) {

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info == null) _info = skill as AxeSkillSO;
        if (_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>();

    }

    public override void OnPreCast(SkillSO skill) {

        // Bloqueando movimentação e outros inputs
        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        movementManager.ChangeRotationType(RotationType.MouseRotation);
        skillManager.BlockSkillInputs(slot, true);

        // Ligar animação
        anim.SetTrigger(_info.FirstAnimationParameterName);

        // Começar o timer
        _chargeTimeCoroutine ??= StartCoroutine(ChargeTimer());

        if (_info.PreCastOn) SetSkillRangeIndicator(skill);
    }

    public override void UseSkill(SkillSO skill) {
        _attackCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator ChargeTimer() {

        _chargeTimer = 0;

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition);

        while (_isHoldingInput || _chargeTimer < _info.MinimalChargeTime) {
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer >= _info.MaxChargeTime) break;
            yield return null; ;
        }

        if (_chargeTimer >= _info.MaxChargeTime) {
            _preCasted = false;
            OnRelease(_info);
        }

        _chargeTimeCoroutine = null;
    }

    IEnumerator Attack() {
        while (_chargeTimer < _info.MinimalChargeTime) yield return null;

        anim.SetTrigger(_info.SecondAnimationParameterName);

        cooldownManager.SetCooldown(slot, _info.Cooldown);

        AnimatorStateInfo stateInfo;

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.SecondAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[0].Sort((a, b) => a.timeToSpawnPreFab.CompareTo(b.timeToSpawnPreFab));

        for (int i = 0; i < _info.Prefabs.Count; i++) {
            SkillAnimationEvent prefabInfo = _info.Prefabs[0][i];
            float targetNormalizedTime = prefabInfo.timeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            GameObject preFab = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.preFabName, prefabInfo.preFab);
            preFab.transform.SetParent(parent.transform);
            preFab.transform.SetLocalPositionAndRotation(prefabInfo.preFabPosition, Quaternion.identity);

            if (prefabInfo.prefabType == TypeOfSkillAnimationPrefab.Hitbox) {

                InstantDamageContext newContext = new(
                    ReturnDamage(),
                    _info.HitBoxDuration,
                    _info.Penetration,
                    ReturnHitShield(),
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>()
                    );

                preFab.GetComponent<InstantDamageHitBox>().Initialize(newContext);

                OnWeaponChange?.Invoke();
            }
        }


        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipRightHand();
        UnblockInputs();
        _attackCoroutine = null;
        OnWeaponChange?.Invoke();
        gameObject.SetActive(false);
    }

    float ReturnDamage() {
        float damage = (_chargeTimer * _info.MaxDamage) / _info.MaxChargeTime;
        return Mathf.Clamp(damage, _info.MinDamage, _info.MaxDamage);
    }

    bool ReturnHitShield() {
        if (_chargeTimer >= _info.MaxChargeTime) return true;
        else return false;
    }

    private void OnDestroy() {
        OnWeaponChange = null;
    }

    #endregion
}
