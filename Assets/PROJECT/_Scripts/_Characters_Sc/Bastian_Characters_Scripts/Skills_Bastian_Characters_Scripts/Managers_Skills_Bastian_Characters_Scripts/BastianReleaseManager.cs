using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianReleaseManager : SkillObjectManager {
    // Components
    BastianReleaseSO _info;

    float _attackSpeedMultiplier;

    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx)
    {
        if (!BastianPassiveManager.Instance.CanShoot)
        {
            return;
        }

        base.HandleInput(skill, ctx);
    }

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) _info = skill as BastianReleaseSO;

        gameObject.SetActive(true);

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, 0, _attackSpeedMultiplier));
    }

    protected override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownWithCharges(slot, _info);
    }
    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    protected override void ThirdFunc() {
        BastianPassiveManager.Instance.LooseHeat(_info.HeatLost);

        _info.SkillSound.Post(parent);
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        statusManager.ChangeStatusMultiplier(StatusType.AttackSpeed, _info.AttackSpeedGain, true, _info.AttackSpeedDuration);

        EndWithUnblockSkills();
    }
}
