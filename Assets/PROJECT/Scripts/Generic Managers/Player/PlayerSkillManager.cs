using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillSlot {
    BaseAttack = 0,
    SkillOne = 1,
    SkillTwo = 2,
    Ultimate = 3,
    Dash = 4
}
public class PlayerSkillManager : MonoBehaviour {
    #region Parameters

    // Components
    [HideInInspector] public Animator Anim;
    [HideInInspector] public PlayerMovementManager MoveManager;
    [HideInInspector] public PlayerSkillCooldownManager CooldownManager;
    [HideInInspector] public EnergyManager EnergyManager;

    // Skills
    PassiveSO _passive;
    CommonSkillSO _dash;
    CommonSkillSO _baseAttackSkill;
    CommonSkillSO _skillOne;
    CommonSkillSO _skillTwo;
    UltimateSkillSO _ultimate;
    SkillSO _currentSkill;

    // Dictionarys
    Dictionary<SkillSlot, bool> _skillAvailable = new();

    // Events
    public event Action OnSkillsSet;
    #endregion

    #region Initialize
    private void Awake() {
        Anim = GetComponentInChildren<Animator>();
        MoveManager = GetComponent<PlayerMovementManager>();
        CooldownManager = GetComponent<PlayerSkillCooldownManager>();
        EnergyManager = GetComponent<EnergyManager>();

        foreach (SkillSlot slot in Enum.GetValues(typeof(SkillSlot))) {
            _skillAvailable[slot] = true; // Todas as skills podem ser usadas
        }
    }

    private void Start() {
        SetSkills();
        StartPassive();
    }

    void SetSkills() {
        PlayerWhiteBoard whiteboard = PlayerWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        _skillOne = SafeGetSkill(() => whiteboard.ReturnSkillOne(selectedCharacter), "SkillOne");
        _skillTwo = SafeGetSkill(() => whiteboard.ReturnSkillTwo(selectedCharacter), "SkillTwo");
        _ultimate = SafeGetSkill(() => whiteboard.ReturnUltimate(selectedCharacter), "Ultimate");
        _passive = SafeGetSkill(() => whiteboard.ReturnPassive(selectedCharacter), "Passive");
        _dash = SafeGetSkill(() => whiteboard.ReturnDash(selectedCharacter), "Dash");
        _baseAttackSkill = SafeGetSkill(() => whiteboard.ReturnBaseAttack(selectedCharacter), "BaseAttack");

        OnSkillsSet?.Invoke();

    }

    T SafeGetSkill<T> (Func<T> getSkillFunc, string skillName) where T: class {
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
        Debug.Log("Start Passive");
        GameObject passiveManager = PoolingManager.Instance.ReturnManagerFromPool(_passive.PassiveName, _passive.PassiveManager.gameObject);
        PassiveSkillManager manager = passiveManager.GetComponent<PassiveSkillManager>();
        manager.OnStart(_passive, this.gameObject);
    }
    #endregion

    #region Inputs
    public void OnBaseAttack(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, _baseAttackSkill, SkillSlot.BaseAttack, () => IsSkillAvailable(SkillSlot.BaseAttack));
    }

    public void OnSkillOne(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, _skillOne, SkillSlot.SkillOne, () => IsSkillAvailable(SkillSlot.SkillOne));
    }

    public void OnSkillTwo(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, _skillTwo, SkillSlot.SkillTwo, () => IsSkillAvailable(SkillSlot.SkillTwo));
    }

    public void OnUltimate(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, _ultimate, SkillSlot.Ultimate, () => IsSkillAvailable(SkillSlot.Ultimate));
    }

    public void OnDash(InputAction.CallbackContext ctx) {
        HandleSkillInput(ctx, _dash, SkillSlot.Dash, () => IsSkillAvailable(SkillSlot.Dash));
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
        GameObject skillManager = PoolingManager.Instance.ReturnManagerFromPool(skill.SkillManagerName, skill.SkillManagerObject.gameObject);
        SkillObjectManager manager = skillManager.GetComponent<SkillObjectManager>();
        manager.OnStart(skill, this.gameObject, slot, ctx);

    }

    public bool IsSkillAvailable(SkillSlot slot) {
        return _skillAvailable.TryGetValue(slot, out bool available) && available;
    }

    private bool IsSkillReady(SkillSlot slot) {
        return CooldownManager.ReturnCooldown(slot) <= 0;
    }

    private bool HaveEnergy() {
        return EnergyManager.HasFullEnergy();
    }
    #endregion

    #region Getter

    public UltimateSkillSO ReturnUltimate() => _ultimate;

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

    #endregion

}
