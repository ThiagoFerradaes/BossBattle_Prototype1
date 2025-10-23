using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyrusSpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusSpearSkillSO _info;
    WeaponManager _weaponManager;

    // Atributes
    int _skillLevel = 0;

    #endregion

    #region Methodss

    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

        if (Keyboard.current.ctrlKey.isPressed) return;

        base.HandleInput(skill, ctx);
    }
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
            animationCoroutine ??= StartCoroutine(Attack());
        }

    }

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusSpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
    }

    IEnumerator Attack() {
        float cooldown = _skillLevel >= 3? _info.Level3Cooldown : _info.Cooldown;
        cooldownManager.SetCooldownSingleCharge(slot, cooldown);
        anim.SetTrigger(_info.SpearAttackTriggerName);

        skillManager.SkillIsInAnimation(true);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        // Ligando a arma
        _weaponManager.OnEquipRightHand(_info.SpearPrefab, _info.SpearName, _info.WeaponPosition, _info.WeaponRotation);

        int attackStateHash = stateInfo.fullPathHash;

        int combo = _skillLevel >= 2 ? 1 : 0;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[combo].Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < _info.Prefabs[combo].Count; i++) {
            SkillAnimationEvent prefabInfo = _info.Prefabs[combo][i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) {
                InstantiateHitBox(prefabInfo);
            }
            else {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);

                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);
                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }

        }

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        skillManager.SkillIsInAnimation(false);
        _weaponManager.OnDesequipRightHand();
        animationCoroutine = null;
        EndWithUnblockSkills();
    }

    void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        float zSize = _skillLevel >= 2 ? _info.Level2Range : _info.Size.z;
        preFab.transform.localScale = new(_info.Size.x, _info.Size.y, zSize);

        preFab.transform.SetParent(parent.transform, false);

        Vector3 pos = new(prefabInfo.PreFabPosition.x, prefabInfo.PreFabPosition.y, zSize);

        preFab.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);

        float penetration = _skillLevel > 2 ? _info.Level3Penetration : 0;

        DamageAtributes atributes = _info.SkillDamageAtributes;
        atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration] = penetration;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
            if (_skillLevel > 0) cooldownManager.ResetCooldown(SkillSlot.Dash);
        };
    }
    #endregion
}
