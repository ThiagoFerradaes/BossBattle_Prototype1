using UnityEngine;

public class CyrusTonfasSkillManager : SkillObjectManager
{
    CyrusTonfasSkillSO _info;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        int comboIndex = BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS ? 1 : 0;
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, comboIndex, GetAttackSpeedMultiplier()));
    }
    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    private void Initialize(SkillSO skill) {
        if (_info  == null) _info = skill as CyrusTonfasSkillSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc() {
        base.FirstFunc();

        energyManager.LooseAllEnergy();
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        Vector3 position = parent.transform.position + prefab.PreFabPosition;

        Vector3 size = BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS ? Vector3.one * _info.SizeUpgrade : _info.Atributes.Size;
        hitbox.transform.localScale = size;

        hitbox.transform.SetPositionAndRotation(position, parent.transform.rotation);

        DamageContext newContext = new(_info.Atributes, statusManager);

        InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
        collider.Initialize(newContext);

        //AK.Wwise.Switch newSwitch = _info.ListOfSwitches[_skillLevel];
        //newSwitch.SetValue(parent);
        //_info.SkillSound.Post(parent);

        collider.OnHit += () => {
            if (BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS) energyManager.GainEnergy(_info.EnergyCost/2);
        };
    }
}
