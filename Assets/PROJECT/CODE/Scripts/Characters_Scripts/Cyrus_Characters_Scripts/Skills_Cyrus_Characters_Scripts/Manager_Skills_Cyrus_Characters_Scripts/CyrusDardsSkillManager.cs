using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CyrusDardsSkillManager : SkillObjectManager {

    CyrusDardsSkillSO _info;

    int _skillLevel;

    Coroutine _dardsRoutine;

    public override void UseSkill(SkillSO skill) {
        base.UseSkill(skill);

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameter, _info.AnimationName, 0));
    }


    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusDardsSkillSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    public override void FirstFunc() {
        base.FirstFunc();

        float cooldown = _skillLevel < 2 ? _info.Cooldown : _info.CooldownLevelTwo;
        cooldownManager.SetCooldownWithCharges(slot, _info, cooldown);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        EndWithUnblockSkills();
    }

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {
        _dardsRoutine ??= StartCoroutine(InstantiateDards(prefab));
    }

    IEnumerator InstantiateDards(SkillAnimationEvent prefab) {
        float amountOfHitboxes = _skillLevel < 3 ? _info.AmountOfDards : _info.AmountOfDardsLevelThree;

        for (int i = 0; i < amountOfHitboxes; i++) {

            GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

            hitbox.transform.localScale = _info.SkillDamageAtributes.Size;
            Vector3 position = parent.transform.position + prefab.PreFabPosition;
            hitbox.transform.SetPositionAndRotation(position, parent.transform.rotation);

            DamageContext newContext = new(_info.SkillDamageAtributes, statusManager);

            ProjectileDamageHitBox projectile = hitbox.GetComponent<ProjectileDamageHitBox>();
            projectile.Initialize(newContext);

            projectile.OnCollision += (Collider collision) => {
                energyManager.GainEnergy(_info.FlatEnergyGainPerHit);

                if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);

                if (_skillLevel > 0) {
                    if (collision.TryGetComponent<StatusManager>(out StatusManager status)) {
                        status.ChangeStatus(StatusType.Defense, _info.AmountOfDefenseDrop/100, false, _info.DefenseDropDuration);
                    }
                }
            };

            if (i < amountOfHitboxes - 1) yield return new WaitForSeconds(_info.TimeBetweenDards);
        }

        _dardsRoutine = null;
    }

}
