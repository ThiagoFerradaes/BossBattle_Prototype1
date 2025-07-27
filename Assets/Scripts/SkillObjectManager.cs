using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillObjectManager : MonoBehaviour
{
    #region Parameters
    bool _preCasted;
    bool _hasStarted;

    protected PlayerSkillManager _skillManager;
    protected PlayerMovementManager _movementManager;
    protected GameObject _parent;

    GameObject _currentSkillRange;
    #endregion

    #region Methods
    public virtual void OnStart(SkillSO skill, GameObject parent, SkillSlot slot, InputAction.CallbackContext ctx)
    {
        if (!_hasStarted)
        {
            _hasStarted = true;
            _skillManager = parent.GetComponent<PlayerSkillManager>();
            _movementManager = parent.GetComponent<PlayerMovementManager>();
            _parent = parent;
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
            _movementManager.BlockDash(skill.BlockDashWhilePreCasting);
            _movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
            _movementManager.ChangeRotationType(RotationType.MouseRotation);
            _skillManager.BlockSkillInputs(slot, true);

            SetSkillRangeIndicator(skill);
        }

        else OnRelease(skill, slot);
    }

    public virtual void OnRelease(SkillSO skill, SkillSlot slot)
    {
        Debug.Log("Release");
        ReleaseSkillRangeIndicator();
        _movementManager.ChangeRotationType(RotationType.MoveRotation);

        UseSkill(skill, slot);
    }

    void SetSkillRangeIndicator(SkillSO skill)
    {
        _currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        _currentSkillRange.transform.SetParent(_parent.transform);
        _currentSkillRange.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        _currentSkillRange.SetActive(true);
    }

    void ReleaseSkillRangeIndicator()
    {
        if (_currentSkillRange == null) return;

        _currentSkillRange.SetActive(false);
        _currentSkillRange.transform.SetParent(SkillPoolingManager.Instance.HitboxContainer);
        _currentSkillRange = null;

    }

    public virtual void UseSkill(SkillSO skill, SkillSlot slot) { }

    #endregion
}
