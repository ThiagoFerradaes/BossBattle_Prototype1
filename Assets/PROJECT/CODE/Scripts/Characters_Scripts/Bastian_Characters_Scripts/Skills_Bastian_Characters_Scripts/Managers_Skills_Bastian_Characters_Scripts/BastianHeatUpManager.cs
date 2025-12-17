using UnityEngine;
using UnityEngine.InputSystem;

public class BastianHeatUpManager : SkillObjectManager
{
    BastianHeatUpSO _info;

    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx)
    {
        if (!BastianPassiveManager.Instance.CanShoot)
        {
            return;
        }

        base.HandleInput(skill, ctx);
    }
    public override void UseSkill(SkillSO skill)
    {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as BastianHeatUpSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc()
    {
        base.FirstFunc();

        // Cooldown
        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    public override void ThirdFunc()
    {
        base.ThirdFunc();

        BastianPassiveManager.Instance.SetHeatToAmount(_info.AmountOfHeatToSetUp);

    }

    public override void FourthFunc()
    {
        base.FourthFunc();

        EndWithUnblockSkills();
    }
}
