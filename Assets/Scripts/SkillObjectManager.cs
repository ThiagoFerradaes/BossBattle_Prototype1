using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillObjectManager : MonoBehaviour
{
    #region Parameters
    bool _preCasted;
    bool _hasStarted;

    protected PlayerSkillManager skillManager;
    protected PlayerMovementManager movementManager;
    protected GameObject parent;
    protected GameObject currentSkillRange;
    protected SkillSlot slot;
    protected Animator anim;
    protected PlayerSkillCooldownManager cooldownManager;

    #endregion

    #region Methods
    public virtual void OnStart(SkillSO skill, GameObject parent, SkillSlot slot, InputAction.CallbackContext ctx)
    {
        if (!_hasStarted)
        {
            _hasStarted = true;
            skillManager = parent.GetComponent<PlayerSkillManager>();
            movementManager = parent.GetComponent<PlayerMovementManager>();
            this.parent = parent;
            this.slot = slot;
            anim = parent.GetComponentInChildren<Animator>();
            cooldownManager = parent.GetComponent<PlayerSkillCooldownManager>();
        }
        
        HandleInput(skill, slot, ctx);
    }
    void HandleInput(SkillSO skill, SkillSlot slot, InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            _preCasted = true;
            //OnPreCastingSkill?.Invoke();
            OnPreCast(skill,slot);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted)
        {
            _preCasted = false;
            //OnSkillRelease?.Invoke();
            OnRelease(skill, slot);
        }
    }
    public virtual void OnPreCast(SkillSO skill, SkillSlot slot)
    {
        if (skill.PreCastOn)
        {
            Debug.Log("Pre Casting");
            movementManager.BlockDash(skill.BlockDashWhilePreCasting);
            movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
            movementManager.ChangeRotationType(RotationType.MouseRotation);
            skillManager.BlockSkillInputs(slot, true);

            SetSkillRangeIndicator(skill);
        }

        else OnRelease(skill, slot);
    }

    public virtual void OnRelease(SkillSO skill, SkillSlot slot)
    {
        Debug.Log("Release");
        ReleaseSkillRangeIndicator();
        movementManager.ChangeRotationType(RotationType.MoveRotation);
        skillManager.BlockSkillInputs(slot, false);

        UseSkill(skill);
    }

    public virtual void SetSkillRangeIndicator(SkillSO skill)
    {
        currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        currentSkillRange.transform.SetParent(parent.transform);
        currentSkillRange.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void ReleaseSkillRangeIndicator()
    {
        if (currentSkillRange == null) return;

        currentSkillRange.SetActive(false);
        currentSkillRange.transform.SetParent(SkillPoolingManager.Instance.HitboxContainer);
        currentSkillRange = null;

    }

    public virtual void UseSkill(SkillSO skill) { }

    #endregion
}
