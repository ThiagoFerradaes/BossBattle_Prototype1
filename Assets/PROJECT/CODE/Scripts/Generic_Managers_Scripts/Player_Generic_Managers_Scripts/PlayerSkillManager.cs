using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillSlot {
    BaseAttack = 0,
    SkillOne = 1,
    SkillTwo = 2,
    Ultimate = 3,
    Dash = 4,
    Passive = 5
}
public class PlayerSkillManager : MonoBehaviour {
    #region Parameters

    // Components
    [HideInInspector] public Animator Anim;
    [HideInInspector] public PlayerMovementManager MoveManager;
    [HideInInspector] public PlayerSkillCooldownManager CooldownManager;
    [HideInInspector] public EnergyManager EnergyManager;
    [HideInInspector] public StunManager StunManager;

    // Skills
    PassiveSO _passive;
    SkillSO _currentSkill;

    // Dictionarys
    Dictionary<SkillSlot, bool> _skillAvailable = new();
    Dictionary<SkillSlot, SkillSO> _skills = new();

    // Events
    public static event Action<Dictionary<SkillSlot, SkillSO>> OnSkillsSet;
    public event Action OnStopSkills;

    // Actions
    Action<bool> _onStun;

    // Booleans
    public bool _isInSkillAnimation;
    #endregion

    #region Initialize
    private void Awake() {
        Anim = GetComponentInChildren<Animator>();
        MoveManager = GetComponent<PlayerMovementManager>();
        CooldownManager = GetComponent<PlayerSkillCooldownManager>();
        EnergyManager = GetComponent<EnergyManager>();
        StunManager = GetComponent<StunManager>();

        foreach (SkillSlot slot in Enum.GetValues(typeof(SkillSlot))) {
            _skillAvailable[slot] = true; // Todas as skills podem ser usadas
        }

        _onStun = (bool isStunned) => {
            _isInSkillAnimation = false; 
            if(isStunned) OnStopSkills?.Invoke(); 
            BlockAllSkills(isStunned); 
        };

    }

    private void Start() {
        SetSkills();
        StartPassive();

        StunManager.OnStun += _onStun;
    }

    private void OnDestroy() {
        StunManager.OnStun -= _onStun;
        OnSkillsSet = null;
        OnStopSkills = null;
    }
    void SetSkills() {
        CurrentSelectedCharacterWhiteBoard whiteboard = CurrentSelectedCharacterWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        _skills[SkillSlot.SkillOne] = SafeGetSkill(() => whiteboard.ReturnSkillOne(selectedCharacter), "SkillOne");
        _skills[SkillSlot.SkillTwo] = SafeGetSkill(() => whiteboard.ReturnSkillTwo(selectedCharacter), "SkillTwo");
        _skills[SkillSlot.Dash] = SafeGetSkill(() => whiteboard.ReturnDash(selectedCharacter), "Dash");
        _skills[SkillSlot.BaseAttack] = SafeGetSkill(() => whiteboard.ReturnBaseAttack(selectedCharacter), "BaseAttack");
        _skills[SkillSlot.Ultimate] = SafeGetSkill(() => whiteboard.ReturnUltimate(selectedCharacter), "Ultimate");
        _passive = SafeGetSkill(() => whiteboard.ReturnPassive(selectedCharacter), "Passive");

        OnSkillsSet?.Invoke(_skills);

    }


    T SafeGetSkill<T>(Func<T> getSkillFunc, string skillName) where T : class {
        try {
            T skill = getSkillFunc();
            if (skill == null) Debug.LogWarning($"{skillName} retornou null.");
            return skill;
        }
        catch (Exception e) {
            Debug.LogWarning($"Erro ao setar {skillName}: {e.Message}");
            return null;
        }
    }
    void StartPassive() {
        GameObject passiveManager = PoolingManager.Instance.ReturnManagerFromPool(_passive.PassiveManager.gameObject);
        PassiveSkillManager manager = passiveManager.GetComponent<PassiveSkillManager>();
        manager.OnStart(_passive, this.gameObject);
    }
    #endregion

    #region Inputs
    public void OnBaseAttack(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, (CommonSkillSO)_skills[SkillSlot.BaseAttack], SkillSlot.BaseAttack, () => IsSkillAvailable(SkillSlot.BaseAttack));
    }

    public void OnSkillOne(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, (CommonSkillSO)_skills[SkillSlot.SkillOne], SkillSlot.SkillOne, () => IsSkillAvailable(SkillSlot.SkillOne));
    }

    public void OnSkillTwo(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, (CommonSkillSO)_skills[SkillSlot.SkillTwo], SkillSlot.SkillTwo, () => IsSkillAvailable(SkillSlot.SkillTwo));
    }

    public void OnUltimate(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, (UltimateSkillSO)_skills[SkillSlot.Ultimate], SkillSlot.Ultimate, () => IsSkillAvailable(SkillSlot.Ultimate));
    }

    public void OnDash(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, (CommonSkillSO)_skills[SkillSlot.Dash], SkillSlot.Dash, () => IsSkillAvailable(SkillSlot.Dash));
    }
    #endregion

    #region Skills
    private void HandleSkillInput(InputAction.CallbackContext ctx, CommonSkillSO skill, SkillSlot slot, Func<bool> canUseCondition) {
        if (ctx.phase == InputActionPhase.Canceled && skill != null && skill.Cancelable) {
            _currentSkill = skill;
            UseSkill(ctx, _currentSkill, slot);
            return;
        }

        if (!canUseCondition() || !IsSkillReady(slot) || Time.timeScale == 0)
            return;

        if (skill != null) {
            _currentSkill = skill;
            UseSkill(ctx, _currentSkill, slot);
        }
    }
    private void HandleSkillInput(InputAction.CallbackContext ctx, UltimateSkillSO skill, SkillSlot slot, Func<bool> canUseCondition) {
        if (ctx.phase == InputActionPhase.Canceled && skill != null && skill.Cancelable) {
            _currentSkill = skill;
            UseSkill(ctx, _currentSkill, slot);
            return;
        }

        if (!canUseCondition() || !HaveEnergy() || Time.timeScale == 0)
            return;

        if (skill != null) {
            _currentSkill = skill;
            UseSkill(ctx, _currentSkill, slot);
        }
    }
    void UseSkill(InputAction.CallbackContext ctx, SkillSO skill, SkillSlot slot) {
        GameObject skillManager = PoolingManager.Instance.ReturnManagerFromPool(skill.SkillManagerObject.gameObject);
        SkillObjectManager manager = skillManager.GetComponent<SkillObjectManager>();
        manager.Initialize(skill, this.gameObject, slot, ctx);

    }

    public bool IsSkillAvailable(SkillSlot slot) {
        return _skillAvailable.TryGetValue(slot, out bool available) && available;
    }

    private bool IsSkillReady(SkillSlot slot) {
        return CooldownManager.ReturnIfCanUseSkill(slot);
    }

    private bool HaveEnergy() {
        return EnergyManager.HasFullEnergy();
    }
    #endregion

    #region Getter

    public UltimateSkillSO ReturnUltimate() => (UltimateSkillSO)_skills[SkillSlot.Ultimate];

    #endregion

    #region Setters

    /// <summary>
    /// Blocks or Unblock the input of a specific skill
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="block"></param>
    public void BlockSpecificSkill(SkillSlot slot, bool block) {
        if (!_skillAvailable.ContainsKey(slot)) return;

        _skillAvailable[slot] = !block;
    }

    /// <summary>
    /// Block or unblock the input of all skills
    /// </summary>
    /// <param name="block"></param>
    public void BlockAllSkills(bool block) {
        var keysList = new List<SkillSlot>(_skillAvailable.Keys);
        foreach (var key in keysList) {
            _skillAvailable[key] = !block;
        }
    }

    /// <summary>
    /// Block or unblock all skills but one
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="block"></param>
    public void BlockAllButOneSkill(SkillSlot slot, bool block) {
        var keysList = new List<SkillSlot>(_skillAvailable.Keys);
        foreach (var key in keysList) {
            if (key != slot) _skillAvailable[key] = !block;
        }
    }

    public void SkillIsInAnimation(bool skillIsInAnimation) => _isInSkillAnimation = skillIsInAnimation;

    public bool ReturnIfIsSkillAnimation() => _isInSkillAnimation;

    #endregion

}
