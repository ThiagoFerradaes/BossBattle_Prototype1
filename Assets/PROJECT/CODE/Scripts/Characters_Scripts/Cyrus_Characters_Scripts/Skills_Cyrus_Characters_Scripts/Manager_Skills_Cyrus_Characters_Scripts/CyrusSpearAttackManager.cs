using UnityEngine;


public class CyrusSpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusSpearSkillSO _info;

    // Atributes
    int _skillLevel = 0;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);

            int combo = _skillLevel >= 2 ? 1 : 0;
            animationCoroutine ??= StartCoroutine(AttackCoroutine(0, combo));
        }

    }

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusSpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
        }

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
    }

    #region Attack Animation & Instantiates

    protected override void FirstFunc() {
        float cooldown = _skillLevel >= 3 ? _info.Level3Cooldown : _info.Cooldown;
        cooldownManager.SetCooldownSingleCharge(slot, cooldown);

        float animationSpeed = _info.ListOfAnimationsInfo[0].AnimationSpeed;
        anim.SetFloat("AttackSpeed", animationSpeed);

        base.FirstFunc();
    }


    protected override void FourthFunc() {
        base.FourthFunc();
        
        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        float zSize = _skillLevel >= 2 ? _info.Level2Range : _info.SkillDamageAtributes.Size.z;
        preFab.transform.localScale = new(_info.SkillDamageAtributes.Size.x, _info.SkillDamageAtributes.Size.y, zSize);

        preFab.transform.SetParent(parent.transform, false);

        Vector3 pos = new(prefabInfo.PreFabPosition.x, prefabInfo.PreFabPosition.y, zSize/2);

        preFab.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);

        preFab.transform.SetParent(null);

        float penetration = _skillLevel > 2 ? _info.Level3Penetration : 0;

        DamageAtributes atributes = new(_info.SkillDamageAtributes);
        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = penetration;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel], _info.ListOfSprites);
            if (_skillLevel > 0) cooldownManager.ResetCooldown(SkillSlot.Dash);
        };
    }

    #endregion


    #endregion
}
