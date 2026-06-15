using UnityEngine;

public class CyrusTonfasSkillManager : SkillObjectManager
{
    CyrusTonfasSkillSO _info;

    int _skillLevel;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        int comboIndex = _skillLevel > 1 ? 1 : 0;
        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, comboIndex, GetAttackSpeedMultiplier()));
    }
    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    private void Initialize(SkillSO skill) {
        if (_info  == null) _info = skill as CyrusTonfasSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    protected override void FirstFunc() {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();
    }

    protected override void ThirdFunc() {
        base.ThirdFunc();

        energyManager.SetCanGainEnergy(true);
    }

    protected override void FourthFunc() {
        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        Vector3 position = parent.transform.position + prefab.PreFabPosition;

        Vector3 size = _skillLevel > 1 ? Vector3.one * _info.SizeLevelTwo : _info.Atributes.Size;
        hitbox.transform.localScale = size;

        hitbox.transform.SetPositionAndRotation(position, parent.transform.rotation);

        DamageAtributes newAtributes = new(_info.Atributes);

        DamageContext newContext = new(newAtributes, statusManager);

        InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
        collider.Initialize(newContext);

        AK.Wwise.Switch newSwitch = _info.ListOfSwitches[_skillLevel];
        newSwitch.SetValue(parent);
        _info.SkillSound.Post(parent);

        //collider.OnHit += () => {
        //    if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel], _info.ListOfSprites);
        //    int newLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
        //    if (newLevel >= 1) energyManager.ChangeMaxEnergy(_info.EnergyCostLevelOne);
        //};
    }
}
