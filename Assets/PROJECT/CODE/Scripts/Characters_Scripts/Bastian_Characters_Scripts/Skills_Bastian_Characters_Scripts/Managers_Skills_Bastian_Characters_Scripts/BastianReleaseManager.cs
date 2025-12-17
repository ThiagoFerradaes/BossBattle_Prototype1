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

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownWithCharges(slot, _info);

        _attackSpeedMultiplier = GetAttackSpeedMultiplier();
        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }
    float GetAttackSpeedMultiplier()
    {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    public override void ThirdFunc() {
        BastianPassiveManager.Instance.LooseHeat(_info.HeatLost);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        statusManager.ChangeStatus(StatusType.AttackSpeed, _info.AttackSpeedGain, true, _info.AttackSpeedDuration);

        EndWithUnblockSkills();

        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);
    }

    public override void InstantiateVFX(SkillAnimationEvent prefab) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFabStatic>().Initialize(prefab.VFXAtribute);
    }
}
