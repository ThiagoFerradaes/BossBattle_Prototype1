using UnityEngine;
using UnityEngine.InputSystem;

public class BastianLastWhisper : SkillObjectManager
{
    BastianLastWhisperSO _info;
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

    private void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as BastianLastWhisperSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc()
    {
        base.FirstFunc();
        energyManager.LooseAllEnergy();
        BastianPassiveManager.Instance.SetCanLooseHeat(false);
    }

    public override void FourthFunc()
    {

        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        GameObject newPreFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        newPreFab.transform.SetParent(parent.transform);
        newPreFab.transform.localScale = _info.Atributes.Size;
        Vector3 pos = new(prefab.PreFabPosition.x, prefab.PreFabPosition.y, _info.Atributes.Size.z / 2);
        newPreFab.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
        newPreFab.transform.SetParent(null);

        float pen = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.SuperHeatArea) ? _info.PenetrationOnSuperHeat : 0;
        float critChance = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.OverHeatArea) ? _info.CritChanceOverHeat : 0;
        float additionalCriDmg = BastianPassiveManager.Instance.ReturnMinHeat(HeatArea.LastOverHeatArea) ? _info.LastOverHeatCritDamage : 0;
        float critDamage = statusManager.ReturnStatusValue(StatusType.CritDamage) + additionalCriDmg;
        float damage = _info.Atributes.Damage + (_info.HeatDamageMultiplier * BastianPassiveManager.Instance.ReturnCurrentHeat());

        DamageAtributes atributes = new(_info.Atributes)
        {
            Damage = damage
        };

        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = pen;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = critChance;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = newPreFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        // Diminuindo vapor
        BastianPassiveManager.Instance.LooseAllHeat();
        BastianPassiveManager.Instance.SetCanLooseHeat(true);
    }
}
