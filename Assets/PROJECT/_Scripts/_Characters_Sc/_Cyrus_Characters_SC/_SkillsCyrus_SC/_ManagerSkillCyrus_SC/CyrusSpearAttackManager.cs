using UnityEngine;


public class CyrusSpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusSpearSkillSO _info;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);

            int combo = BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS ? 1 : 0;
            animationCoroutine ??= StartCoroutine(AttackCoroutine(0, combo));
        }

    }

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusSpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
        }

        //_skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
    }

    #region Attack Animation & Instantiates

    protected override void FirstFunc() {
        float cooldown = BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS ? _info.UpgradeCooldown : _info.Cooldown;
        cooldownManager.SetCooldownSingleCharge(slot, cooldown);

        base.FirstFunc();
    }


    protected override void FourthFunc() {
        base.FourthFunc();
        
        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        float zSize = BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS ? _info.UpgradeRange : _info.SkillDamageAtributes.Size.z;
        preFab.transform.localScale = new(_info.SkillDamageAtributes.Size.x, _info.SkillDamageAtributes.Size.y, zSize);

        preFab.transform.SetParent(parent.transform, false);

        Vector3 pos = new(prefabInfo.PreFabPosition.x, prefabInfo.PreFabPosition.y, zSize/2);

        preFab.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);

        preFab.transform.SetParent(null);

        DamageContext newContext = new(
            _info.SkillDamageAtributes,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        //AK.Wwise.Switch newSwitch = _info.ListOfSwitches[_skillLevel];
        //newSwitch.SetValue(parent);
        //_info.SkillSound.Post(parent);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }

    #endregion


    #endregion
}
