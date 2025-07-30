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
        }
        this.slot = slot;
        HandleInput(skill, ctx);
    }
    public virtual void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            //OnPreCastingSkill?.Invoke();
            OnPreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            _preCasted = false;
            //OnSkillRelease?.Invoke();
            OnRelease(skill);
        }
    }
    public virtual void OnPreCast(SkillSO skill) {

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        skillManager.BlockSkillInputs(slot, true);

        if (skill.PreCastOn) {

            movementManager.ChangeRotationType(RotationType.MouseRotation);

            SetSkillRangeIndicator(skill);
        }

        else OnRelease(skill);
    }

    public virtual void OnRelease(SkillSO skill) {

        ReleaseSkillRangeIndicator();
        movementManager.ChangeRotationType(RotationType.MoveRotation);

        UseSkill(skill);
    }

    public virtual void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        currentSkillRange.transform.SetParent(parent.transform);
        currentSkillRange.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void ReleaseSkillRangeIndicator() {
        if (currentSkillRange == null) return;

        currentSkillRange.SetActive(false);
        currentSkillRange.transform.SetParent(SkillPoolingManager.Instance.HitboxContainer);
        currentSkillRange = null;

    }
    public virtual void UnblockInputs() {

        skillManager.MoveManager.BlockWalk(false);
        skillManager.BlockSkillInputs(slot, false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }
    public virtual void UseSkill(SkillSO skill) { }

    public virtual float ReturnSkillDamage(float skillDamage) {
        float rng = Random.Range(0, 100);

        if (rng > statusManager.ReturnStatusValue(StatusType.CritRate)) // Não critou
            return statusManager.ReturnStatusValue(StatusType.Attack) * skillDamage;
        else // Critou
            return statusManager.ReturnStatusValue(StatusType.Attack) * skillDamage 
                * statusManager.ReturnStatusValue(StatusType.CritDamage)/100;
    }

    #endregion
}
