using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BastianIgnisManager : SkillObjectManager {
    // Components
    BastianIgnisSO _info;

    float _attackSpeedMultiplier;
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (!BastianPassiveManager.Instance.CanShoot) {
            return;
        }

        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            PreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            ReleaseInput(skill);
        }
    }

    public override void UseSkill(SkillSO skill) {


        if (_info == null) _info = skill as BastianIgnisSO;

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        cooldownManager.SetCooldownWithCharges(slot, _info);
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();

        skillManager.SkillIsInAnimation(true);

        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }

    public override void FourthFunc() {
        // Resetando a velocidade da animação
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        // Corrotina
        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        EndWithUnblockSkills();
    }
    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.Size;
        preFab.transform.SetPositionAndRotation(parent.transform.position + prefabInfo.PreFabPosition, parent.transform.rotation);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.LastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;

        DamageAtributes atributes = new(_info.SkillDamageAtributes);
        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = critChance;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;
        
        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = preFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        if (BastianPassiveManager.Instance.ReturnMaxHeat(HeatArea.SuperHeatArea))
            BastianPassiveManager.Instance.GainHeat(_info.HeatGain);
        else BastianPassiveManager.Instance.GainHeat(1);
    }

    public override void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFabStatic>().Initialize(prefabInfo.VFXDuration);
    }
}
