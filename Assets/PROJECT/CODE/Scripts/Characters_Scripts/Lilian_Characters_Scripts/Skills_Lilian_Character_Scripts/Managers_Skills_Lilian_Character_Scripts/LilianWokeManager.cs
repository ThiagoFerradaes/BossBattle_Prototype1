using UnityEngine;

public class LilianWokeManager : SkillObjectManager
{
    LilianWokeSO _info;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as LilianWokeSO;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownSingleCharge(slot, _info.Cooldown);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefabinfo) {
        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(prefabinfo.PreFab, TypeOfSkillPrefab.Hitbox);

        prefab.transform.localScale = _info.SkillDamageAtributes.Size;
        prefab.transform.SetPositionAndRotation(parent.transform.position, parent.transform.rotation);

        DamageContext newContext = new(
            _info.SkillDamageAtributes,
            parent.GetComponent<StatusManager>()
            );

        ContinuosDamageHitBox hitbox = prefab.GetComponent<ContinuosDamageHitBox>();
        hitbox.Initialize(newContext);
    }
}
