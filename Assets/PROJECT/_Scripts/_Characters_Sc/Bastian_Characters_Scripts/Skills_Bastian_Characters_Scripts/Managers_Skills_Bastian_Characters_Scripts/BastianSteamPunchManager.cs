using UnityEngine;
using UnityEngine.InputSystem;

public class BastianSteamPunchManager : SkillObjectManager
{
    BastianSteamPunchSO _info;
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
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine());
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as BastianSteamPunchSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc() {

        base.FirstFunc();

        // Cooldown
        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);

        // Pegando velocidade de ataque
        _attackSpeedMultiplier = GetAttackSpeedMultiplier();

        anim.SetFloat(_info.AttackSpeedAnimationParameter, _attackSpeedMultiplier);
    }
    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    protected override void FourthFunc() {
        // Resetando a velocidade da anima��o
        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {
        GameObject newPreFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        newPreFab.transform.localScale = _info.SkillDamageAtributes.Size;
        newPreFab.transform.SetParent(parent.transform);
        newPreFab.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);
        newPreFab.transform.SetParent(null);

        //float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;

        DamageAtributes atributes = new(_info.SkillDamageAtributes);
        //atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        atributes.Speed *= _attackSpeedMultiplier;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        ProjectileDamageHitBox hitbox = newPreFab.GetComponent<ProjectileDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };

        // Diminuindo vapor
        BastianPassiveManager.Instance.LooseHeat(_info.HeatLoss);
    }
}
