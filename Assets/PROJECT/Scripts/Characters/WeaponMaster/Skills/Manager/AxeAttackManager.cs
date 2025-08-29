using System;
using System.Collections;
using UnityEditor.Rendering.LookDev;
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
        skillManager.BlockAllSkills(true);

        // Ligar animação
        anim.SetTrigger(_info.FirstAnimationParameterName);

        // Começar o timer
        _chargeTimeCoroutine ??= StartCoroutine(ChargeTimer());

        // Começar o cooldown
        cooldownManager.SetCooldown(slot, _info.Cooldown);

        if (_info.PreCastOn) SetSkillRangeIndicator(skill);
    }

    public override void UseSkill(SkillSO skill) {
        _attackCoroutine ??= StartCoroutine(Attack());
    }

    IEnumerator ChargeTimer() {

        _chargeTimer = 0;

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition, _info.WeaponRotation);

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

        AnimatorStateInfo stateInfo;

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.SecondAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[0].Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < _info.Prefabs[0].Count; i++) {
            SkillAnimationEvent prefabInfo = _info.Prefabs[0][i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) {

                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                float damage = ReturnDamage();
                InstantDamageContext newContext = new(
                    damage,
                    damage,
                    prefabInfo.PrefabDuration,
                    _info.Penetration,
                    true,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>()
                    );

                preFab.GetComponent<InstantDamageHitBox>().Initialize(newContext, ReturnBreakShield());

                OnWeaponChange?.Invoke();
            }
            else {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);
                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
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

    bool ReturnBreakShield() {
        if (_chargeTimer >= _info.MaxChargeTime) return true;
        else return false;
    }


    private void OnDestroy() {
        OnWeaponChange = null;
    }

    #endregion
}
