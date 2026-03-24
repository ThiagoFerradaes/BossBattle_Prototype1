using UnityEngine;
using UnityEngine.InputSystem;

public class BastianHeatUpManager : SkillObjectManager
{
    BastianHeatUpSO _info;
    float _attackSpeedMultiplier;
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

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as BastianHeatUpSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc()
    {
        base.FirstFunc();

        // Cooldown
        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }
    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    protected override void ThirdFunc()
    {
        base.ThirdFunc();

        GainHeat();

    }

    void GainHeat()
    {
        float currentHeat = BastianPassiveManager.Instance.ReturnCurrentHeat();

        if (currentHeat < _info.AmountOfHeatToSetUp) BastianPassiveManager.Instance.SetHeatToAmount(_info.AmountOfHeatToSetUp);
        else if (BastianPassiveManager.Instance.ReturnMaxHeat(HeatArea.SuperHeatArea)) BastianPassiveManager.Instance.GainHeat(_info.ExtraAmountOfHeat);
    }

    protected override void FourthFunc()
    {
        base.FourthFunc();

        // Resetando a velocidade da anima��o
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        EndWithUnblockSkills();
    }
}
