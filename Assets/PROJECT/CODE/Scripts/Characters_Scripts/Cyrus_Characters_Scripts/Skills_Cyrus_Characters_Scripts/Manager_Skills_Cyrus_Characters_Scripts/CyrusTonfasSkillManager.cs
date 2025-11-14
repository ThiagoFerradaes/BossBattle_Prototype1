using UnityEngine;

public class CyrusTonfasSkillManager : SkillObjectManager
{
    CyrusTonfasSkillSO _info;

    int _skillLevel;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    private void Initialize(SkillSO skill) {
        if (_info  == null) _info = skill as CyrusTonfasSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();

        float animationSpeed = _skillLevel > 1 ? _info.AnimationSpeedLevelTwo : 1;
        anim.SetFloat(_info.AnimationSpeedParameter, animationSpeed);
    }

    public override void ThirdFunc() {
        base.ThirdFunc();

        energyManager.SetCanGainEnergy(true);
    }

    public override void FourthFunc() {
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

        if (_skillLevel >= 3) {
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.CritRateLevelThree/100;
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = _info.CritDamageLevelThree/100;
        }


        DamageContext newContext = new(newAtributes, statusManager);

        InstantDamageHitBox collider = hitbox.GetComponent<InstantDamageHitBox>();
        collider.Initialize(newContext);

        collider.OnHit += () => {
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
            int newLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
            if (newLevel >= 1) energyManager.ChangeMaxEnergy(_info.EnergyCostLevelOne);
        };
    }
}
