using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillManager : MonoBehaviour
{
    // Booleans
    bool _canBaseAttack;
    bool _canUseCommonSkill;
    bool _canUseSupreme;
    bool _canUseAnySkill;

    // Components
    Animator anim;
    PlayerMovementManager moveManager;

    // Skills
    [Header("Skills")]
    [SerializeField] SkillSO baseAttackSkill;
    [SerializeField] SkillSO skillOne;
    [SerializeField] SkillSO skillTwo;
    [SerializeField] SkillSO ultimate;
    SkillSO _currentSkill;


    #region Initialize
    private void Awake() {
        anim = GetComponentInChildren<Animator>();
        moveManager = GetComponent<PlayerMovementManager>();
    }
    #endregion

    #region Inputs
    public void OnBasAttack(InputAction.CallbackContext ctx) {
        if (!_canBaseAttack || !_canUseAnySkill) return;

        if (baseAttackSkill != null) {
            _currentSkill = baseAttackSkill;
            baseAttackSkill.UseSKill(anim);
        }
    }
    public void OnSkillOne(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill) return;

        if (skillOne != null) {
            _currentSkill = skillOne;
            skillOne.UseSKill(anim);
        }
    }
    public void OnSkillTwo(InputAction.CallbackContext ctx) {
        if (!_canUseCommonSkill || !_canUseAnySkill) return;

        if (skillTwo != null) {
            _currentSkill = skillTwo;
            skillTwo.UseSKill(anim);
        }
    }
    public void OnUltimate(InputAction.CallbackContext ctx) {
        if (!_canUseSupreme || !_canUseAnySkill) return;

        if (ultimate != null) {
            _currentSkill = ultimate;
            ultimate.UseSKill(anim);
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
    public void BlockSupreme(bool block) => _canUseSupreme = !block;
    /// <summary>
    /// block true = cant attack, block false = can attack
    /// </summary>
    /// <param name="block"></param>
    public void BlockAnySkill(bool block) => _canUseAnySkill = !block;
    #endregion

    public SkillSO ReturnCurrentSkill() => _currentSkill;
}
