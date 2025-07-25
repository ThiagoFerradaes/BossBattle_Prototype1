using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillSlot {
    BaseAttack = 0,
    SkillOne = 1,
    SkillTwo = 2,
    Ultimate = 3
}
public class PlayerSkillManager : MonoBehaviour {
    #region Parameters
    // Booleans
    bool _canBaseAttack = true;
    bool _canUseCommonSkill = true;
    bool _canUseCommonSkillOne = true;
    bool _canUseCommonSkillTwo = true;
    bool _canUseSupreme = true;
    bool _canUseAnySkill = true;
    bool _preCasted;

    // Components
    [HideInInspector] public Animator anim;
    [HideInInspector] public PlayerMovementManager moveManager;

    // Skills
    [Header("Skills")]
    [SerializeField] SkillSO baseAttackSkill;
    [SerializeField] SkillSO skillOne;
    [SerializeField] SkillSO skillTwo;
    [SerializeField] SkillSO ultimate;
    SkillSO _currentSkill;
    GameObject _currentSkillRange;
    List<float> _listOfCooldowns = new() { 0, 0, 0, 0};

    // Events
    public event Action OnPreCastingSkill;
    public event Action OnSkillRelease;

    #endregion

    #region Initialize
    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        moveManager = GetComponent<PlayerMovementManager>();
    }
    #endregion

    #region Inputs
    public void OnBasAttack(InputAction.CallbackContext ctx) {
        if (!_canBaseAttack || !_canUseAnySkill || _listOfCooldowns[(int)SkillSlot.BaseAttack] > 0) return;

        if (baseAttackSkill != null) {
            _currentSkill = baseAttackSkill;
            UseSkill(ctx, _currentSkill, SkillSlot.BaseAttack);
        }
    }
    public void OnSkillOne(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill || !_canUseCommonSkillOne || _listOfCooldowns[(int)SkillSlot.SkillOne] > 0) return;

        if (skillOne != null) {
            _currentSkill = skillOne;
            UseSkill(ctx, _currentSkill, SkillSlot.SkillOne);
        }
    }
    public void OnSkillTwo(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill || !_canUseCommonSkillTwo || _listOfCooldowns[(int)SkillSlot.SkillTwo] > 0) return;

        if (skillTwo != null) {
            _currentSkill = skillTwo;
            UseSkill(ctx, _currentSkill, SkillSlot.SkillTwo);
        }
    }
    public void OnUltimate(InputAction.CallbackContext ctx) {
        if (!_canUseSupreme || !_canUseAnySkill || _listOfCooldowns[(int)SkillSlot.Ultimate] > 0) return;

        if (ultimate != null) {
            _currentSkill = ultimate;
            UseSkill(ctx, _currentSkill, SkillSlot.Ultimate);
        }
    }
    #endregion

    #region Skills
    void UseSkill(InputAction.CallbackContext ctx, SkillSO skill, SkillSlot slot) {
        Debug.Log("Input taken");
        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            OnPreCastingSkill?.Invoke();
            PreCastingSkill(skill, slot);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            _preCasted = false;
            OnSkillRelease?.Invoke();
            ReleaseSkill(skill, slot);
        }
    }
    void PreCastingSkill(SkillSO skill, SkillSlot slot) {
        Debug.Log("Pre Casting");
        moveManager.BlockDash(skill.BlockDashWhilePreCasting);
        moveManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        moveManager.ChangeRotationType(RotationType.MouseRotation);
        BlockSkillInputs(slot, true);

        SetSkillRangeIndicator(skill);
    }
    void ReleaseSkill(SkillSO skill, SkillSlot slot) {
        Debug.Log("Release");
        ReleaseSkillRangeIndicator();
        moveManager.ChangeRotationType(RotationType.MoveRotation);

        GameObject skillManager = SkillPoolingManager.Instance.ReturnManagerFromPool(skill.SkillManagerName, skill.SkillManagerObject.gameObject);
        SkillObjectManager manager = skillManager.GetComponent<SkillObjectManager>();
        manager.OnStart(skill, this, anim, slot);
    }

    void SetSkillRangeIndicator(SkillSO skill) {
        _currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        _currentSkillRange.transform.SetParent(this.transform);
        _currentSkillRange.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        _currentSkillRange.SetActive(true);
    }

    void ReleaseSkillRangeIndicator() {
        if (_currentSkillRange == null) return;

        _currentSkillRange.SetActive(false);
        _currentSkillRange.transform.SetParent(null);
        _currentSkillRange = null;

    }

    public void BlockSkillInputs(SkillSlot slot, bool block) {
        switch (slot) {
            case SkillSlot.BaseAttack:
                BlockCommonSkill(block);
                BlockUltimate(block);
                break;
            case SkillSlot.SkillOne:
                BlockBaseAttack(block);
                BlockCommonSkillTwo(block);
                BlockUltimate(block);
                break;
            case SkillSlot.SkillTwo:
                BlockBaseAttack(block);
                BlockCommonSkillOne(block);
                BlockUltimate(block);
                break;
            case SkillSlot.Ultimate:
                BlockBaseAttack(block);
                BlockCommonSkill(block);
                break;
        }
    }
    #endregion

    #region Setters

    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockBaseAttack(bool block) => _canBaseAttack = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockCommonSkill(bool block) => _canUseCommonSkill = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockCommonSkillOne(bool block) => _canUseCommonSkillOne = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockCommonSkillTwo(bool block) => _canUseCommonSkillTwo = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockUltimate(bool block) => _canUseSupreme = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockAnySkill(bool block) => _canUseAnySkill = !block;
    #endregion

    #region Getters
    public SkillSO ReturnCurrentSkill() => _currentSkill;
    #endregion

    public void SetCooldown(SkillSlot slot, float cooldown) {
        _listOfCooldowns[(int)slot] = cooldown;
        StartCoroutine(DecreaseCooldown(slot));
    }

    IEnumerator DecreaseCooldown(SkillSlot slot) {
        while (_listOfCooldowns[(int)slot] > 0) {
            _listOfCooldowns[(int)slot] -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Cooldown ended");
    }
}
