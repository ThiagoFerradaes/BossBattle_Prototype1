using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillObjectManager : MonoBehaviour {
    #region Parameters
    protected bool _preCasted;
    bool _hasStarted;

    protected PlayerSkillManager skillManager;
    protected PlayerMovementManager movementManager;
    protected GameObject parent;
    protected GameObject currentSkillRange;
    protected SkillSlot slot;
    protected Animator anim;
    protected PlayerSkillCooldownManager cooldownManager;
    protected StatusManager statusManager;
    protected EnergyManager energyManager;
    protected Coroutine animationCoroutine;

    Action _stopSkill;

    #endregion

    #region Methods
    public virtual void OnStart(SkillSO skill, GameObject parent, SkillSlot slot, InputAction.CallbackContext ctx) {
        if (!_hasStarted) {
            _hasStarted = true;
            skillManager = parent.GetComponent<PlayerSkillManager>();
            movementManager = parent.GetComponent<PlayerMovementManager>();
            this.parent = parent;
            anim = parent.GetComponentInChildren<Animator>();
            cooldownManager = parent.GetComponent<PlayerSkillCooldownManager>();
            statusManager = parent.GetComponent<StatusManager>();
            energyManager = parent.GetComponent<EnergyManager>();
        }
        this.slot = slot;
        HandleInput(skill, ctx);

        _stopSkill = () => CancelSkill();

        skillManager.OnStopSkills += _stopSkill;
    }
    public virtual void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            OnRelease(skill);
        }
    }
    public virtual void OnPreCast(SkillSO skill) {

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        skillManager.BlockAllButOneSkill(slot, true);

        if (skill.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) {

            movementManager.ChangeRotationType(RotationType.MouseRotation);

            SetSkillRangeIndicator(skill);
        }

        else {

            movementManager.RotateMouse(false);

            OnRelease(skill);
        }
    }

    public virtual void OnRelease(SkillSO skill) {

        _preCasted = false;
        ReleaseSkillRangeIndicator();
        skillManager.BlockAllSkills(true);
        movementManager.ChangeRotationType(RotationType.MoveRotation);

        UseSkill(skill);
    }

    public virtual void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = PoolingManager.Instance.ReturnPrefabFromPool(skill.SkillObjectRangeName,
            skill.SkillObjectRangeObject, TypeOfSkillPrefab.PreCastRange);

        currentSkillRange.transform.SetParent(parent.transform);

        float groundY = FindGroundHeight(parent.transform.position);
        Vector3 groundPos = new(0, groundY - parent.transform.position.y, 0);

        currentSkillRange.transform.SetLocalPositionAndRotation(groundPos, Quaternion.identity);

        currentSkillRange.SetActive(true);
    }

    float FindGroundHeight(Vector3 originalPos) {
        Vector3 startPos = originalPos + Vector3.up * 0.5f;

        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.5f;
        }

        return 0f;
    }

    void ReleaseSkillRangeIndicator() {
        if (currentSkillRange == null) return;

        PoolingManager.Instance.ReturnObjectToPool(currentSkillRange, TypeOfSkillPrefab.PreCastRange);
        currentSkillRange = null;

    }
    public virtual void UnblockInputs() {

        skillManager.MoveManager.BlockWalk(false);
        skillManager.BlockAllButOneSkill(slot, false);
        skillManager.BlockAllSkills(false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }
    public virtual void UseSkill(SkillSO skill) { }

    public virtual void End() {
        if(!skillManager.ReturnIfIsSkillAnimation()) UnblockInputs();

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Manager);

        skillManager.OnStopSkills -= _stopSkill;
    }

    public virtual void CancelSkill() {
        StopAllCoroutines();
        ReleaseSkillRangeIndicator();

        _preCasted = false;
        skillManager.SkillIsInAnimation(false);

        animationCoroutine = null;

        End();
    }

    #endregion
}
