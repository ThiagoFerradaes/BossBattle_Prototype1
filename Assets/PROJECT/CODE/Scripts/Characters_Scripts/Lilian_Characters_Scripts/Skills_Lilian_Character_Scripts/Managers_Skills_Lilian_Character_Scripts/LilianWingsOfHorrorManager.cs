using System.Collections;
using UnityEngine;

public class LilianWingsOfHorrorManager : SkillObjectManager {
    // Components
    LilianWingsOfHorrorSO _info;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        if (_info == null) _info = skill as LilianWingsOfHorrorSO;

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }

    public override void FirstFunc() {
        base.FirstFunc();

        cooldownManager.SetCooldownWithCharges(slot, _info);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {
        GameObject skull = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        skull.transform.SetParent(parent.transform);
        skull.transform.localPosition = prefab.PreFabPosition;

        skull.transform.SetParent(null);
        skull.transform.LookAt(parent.transform.position);

        skull.GetComponent<LilianWingsOfHorrorObject>().Initialize(statusManager, _info, healthManager, energyManager);

    }
}
