using System;
using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ShootUpUltimateManager : SkillObjectManager {
    #region Parameter

    // Components
    ShootUpUltimateSO _info;
    WeaponManager _weaponManager;
    EnergyManager _energyManager;

    // Events
    public static event Action OnWeaponChange;

    #endregion

    #region Methods

    public override void UseSkill(SkillSO skill) {

        Initialize(skill);
        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
            animationCoroutine ??= StartCoroutine(Attack());
        }

    }

    public override void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = PoolingManager.Instance.ReturnPrefabFromPool(skill.SkillObjectRangeName,
            skill.SkillObjectRangeObject, TypeOfSkillPrefab.PreCastRange);

        currentSkillRange.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as ShootUpUltimateSO;

        if (_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>(); 
        if (_energyManager == null) _energyManager = parent.GetComponent<EnergyManager>();
    }


    IEnumerator Attack() {
        _energyManager.LooseAllEnergy();
        anim.SetTrigger(_info.AnimationParameterTrigger);

        skillManager.SkillIsInAnimation(true);

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition, _info.WeaponOneRotation);
        _weaponManager.OnEquipLeftHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponTwoPosition, _info.WeaponTwoRotation);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[0].Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < _info.Prefabs[0].Count; i++) {
            SkillAnimationEvent prefabInfo = _info.Prefabs[0][i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) {

                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
                preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                DamageContext newContext = new(
                    _info.MinDamage,
                    _info.MaxDamage,
                    prefabInfo.PrefabDuration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>(),
                    new() {
                        {ExtraDamageContextAtributes.DamageCooldown, _info.DamageCooldown }
                    }
                    );
                preFab.GetComponent<ContinuosDamageHitBox>().Initialize(newContext);

                OnWeaponChange?.Invoke();
            }

            else {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

                preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }
        }

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.LastAnimationName));

        attackStateHash = stateInfo.fullPathHash;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipLeftHand();
        _weaponManager.OnDesequipRightHand();

        skillManager.SkillIsInAnimation(false);

        animationCoroutine = null;
        OnWeaponChange?.Invoke();
        End();
    }

    private void OnDestroy() {
        OnWeaponChange = null;
    }
    #endregion
}
