using System.Collections;
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

        switch (ctx.phase) {
            case InputActionPhase.Started:
                _preCasted = true;
                _isHoldingInput = true;
                OnPreCast(skill);
                break;

            case InputActionPhase.Canceled when _preCasted:
                _preCasted = false;
                _isHoldingInput = false;
                OnRelease(skill);
                break;
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
            _chargeTimeCoroutine = StartCoroutine(ChargeTimer());

            if (_info.PreCastOn) SetSkillRangeIndicator(skill);
    }

    public override void UseSkill(SkillSO skill) {
        _attackCoroutine = StartCoroutine(Attack());
    }

    IEnumerator ChargeTimer() {

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

        AnimatorStateInfo stateInfo;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.SecondAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        SkillAnimationEvent prefabInfo = _info.Prefabs[0];
        float targetNormalizedTime = prefabInfo.timeToSpawnHitBox;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.hitboxName, prefabInfo.hitboxPrefab);
        attackHitBox.transform.SetParent(parent.transform);
        attackHitBox.transform.SetLocalPositionAndRotation(_info.HitBoxPosition, Quaternion.identity);
        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(ReturnDamage(), 0.1f);

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        UnblockInputs();
        _attackCoroutine = null;
        gameObject.SetActive(false);
    }

    float ReturnDamage() {
        float damage = (_chargeTimer * _info.MaxDamage) / _info.MaxChargeTime;
        return Mathf.Clamp(damage, _info.BaseDamage, _info.MaxDamage);
    }
}
