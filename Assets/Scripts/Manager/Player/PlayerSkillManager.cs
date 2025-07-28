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
    // Booleans
    bool _canBaseAttack = true;
    bool _canUseCommonSkill = true;
    bool _canUseCommonSkillOne = true;
    bool _canUseCommonSkillTwo = true;
    bool _canUseSupreme = true;
    bool _canUseAnySkill = true;

    // Components
    [HideInInspector] public Animator Anim;
    [HideInInspector] public PlayerMovementManager MoveManager;
    [HideInInspector] public PlayerSkillCooldownManager CooldownManager;

    // Skills
    [Header("Skills")]
    [SerializeField] SkillSO baseAttackSkill;
    [SerializeField] SkillSO skillOne;
    [SerializeField] SkillSO skillTwo;
    [SerializeField] SkillSO ultimate;
    [SerializeField] SkillSO dash;
    SkillSO _currentSkill;

    #endregion

    #region Initialize
    private void Awake() {
        Anim = GetComponentInChildren<Animator>();
        MoveManager = GetComponent<PlayerMovementManager>();
        CooldownManager = GetComponent<PlayerSkillCooldownManager>();
    }
    #endregion

    #region Inputs
    public void OnBasAttack(InputAction.CallbackContext ctx) {
        if (!_canBaseAttack || !_canUseAnySkill || CooldownManager.ReturnCooldown(SkillSlot.BaseAttack) > 0) return;

        if (baseAttackSkill != null) {
            _currentSkill = baseAttackSkill;
            UseSkill(ctx, _currentSkill, SkillSlot.BaseAttack);
        }
    }
    public void OnSkillOne(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill || !_canUseCommonSkillOne || CooldownManager.ReturnCooldown(SkillSlot.SkillOne) > 0) return;

        if (skillOne != null) {
            _currentSkill = skillOne;
            UseSkill(ctx, _currentSkill, SkillSlot.SkillOne);
        }
    }
    public void OnSkillTwo(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill || !_canUseCommonSkillTwo || CooldownManager.ReturnCooldown(SkillSlot.SkillTwo) > 0) return;

        if (skillTwo != null) {
            _currentSkill = skillTwo;
            UseSkill(ctx, _currentSkill, SkillSlot.SkillTwo);
        }
    }
    public void OnUltimate(InputAction.CallbackContext ctx) {
        if (!_canUseSupreme || !_canUseAnySkill || CooldownManager.ReturnCooldown(SkillSlot.Ultimate) > 0) return;

        if (ultimate != null) {
            _currentSkill = ultimate;
            UseSkill(ctx, _currentSkill, SkillSlot.Ultimate);
        }
    }
    public void OnDash(InputAction.CallbackContext ctx) {
        if (!MoveManager.ReturnCanDash() || !_canUseAnySkill || CooldownManager.ReturnCooldown(SkillSlot.Dash) > 0) return;

        if (dash != null) {
            _currentSkill = dash;
            UseSkill(ctx, _currentSkill, SkillSlot.Dash);
        }
    }
    #endregion

    #region Skills
    void UseSkill(InputAction.CallbackContext ctx, SkillSO skill, SkillSlot slot) {

        GameObject skillManager = SkillPoolingManager.Instance.ReturnManagerFromPool(skill.SkillManagerName, skill.SkillManagerObject.gameObject);
        SkillObjectManager manager = skillManager.GetComponent<SkillObjectManager>();
        manager.OnStart(skill, this.gameObject, slot, ctx);

    }

    public void BlockSkillInputs(SkillSlot slot, bool block) {
        switch (slot) {
            case SkillSlot.BaseAttack:
                BlockCommonSkill(block);
                BlockUltimate(block);
                BlockDash(block);
                break;
            case SkillSlot.SkillOne:
                BlockBaseAttack(block);
                BlockCommonSkillTwo(block);
                BlockUltimate(block);
                BlockDash(block);
                break;
            case SkillSlot.SkillTwo:
                BlockBaseAttack(block);
                BlockCommonSkillOne(block);
                BlockUltimate(block);
                BlockDash(block);
                break;
            case SkillSlot.Ultimate:
                BlockBaseAttack(block);
                BlockCommonSkill(block);
                BlockDash(block);
                break;
            case SkillSlot.Dash:
                BlockBaseAttack(block);
                BlockCommonSkill(block);
                BlockUltimate(block);
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
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockDash(bool block) => MoveManager.BlockDash(block);
    #endregion

    #region Getters
    public SkillSO ReturnCurrentSkill() => _currentSkill;

    #endregion

}
