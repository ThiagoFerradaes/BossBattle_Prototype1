using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeAttackManager : SkillObjectManager {

    // Components
    AxeSkillSO _info;

    // Booleans
    bool _isHoldingInput;

    // Floats
    float _chargeTimer;

    // Coroutine
    Coroutine _chargeTimeCoroutine;
    Coroutine _attackCoroutine;
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

        Initialize(skill);

        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            _isHoldingInput = true;
            //OnPreCastingSkill?.Invoke();
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            _preCasted = false;
            _isHoldingInput = false;
            //OnSkillRelease?.Invoke();
            OnRelease(skill);
        }
    }

    void Initialize(SkillSO skill) {

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info == null) _info = skill as AxeSkillSO;

    }

    public override void OnPreCast(SkillSO skill) {

            Debug.Log("Pre Casting");

            // Bloqueando movimentação e outros inputs
            movementManager.BlockDash(skill.BlockDashWhilePreCasting);
            movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
            movementManager.ChangeRotationType(RotationType.MouseRotation);
            skillManager.BlockSkillInputs(slot, true);

            // Ligar animação
            anim.SetTrigger(_info.FirstAnimationParameterName);

            // Começar o timer
            _chargeTimeCoroutine = StartCoroutine(ChargeTImer());

            if (_info.PreCastOn) SetSkillRangeIndicator(skill);
    }

    public override void UseSkill(SkillSO skill) {
        _attackCoroutine = StartCoroutine(Attack());
    }

    IEnumerator ChargeTImer() {

        _chargeTimer = 0;

        cooldownManager.SetCooldown(slot, _info.Cooldown);

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

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsName(_info.SecondAnimationName)) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < targetNormalizedTime) {
            yield return null;
        }

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(ReturnDamage(), 0.1f);

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockMove();
        _attackCoroutine = null;
        gameObject.SetActive(false);
    }

    float ReturnDamage() {
        float damage = (_chargeTimer * _info.MaxDamage) / _info.MaxChargeTime;
        return Mathf.Clamp(damage, _info.BaseDamage, _info.MaxDamage);
    }
    void UnblockMove() {
        Debug.Log("UnblockMove");
        skillManager.MoveManager.BlockDash(false);
        skillManager.MoveManager.BlockWalk(false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }
}
