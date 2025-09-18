using System;
using System.Collections;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusSpearSkillSO _info;
    WeaponManager _weaponManager;

    // Event
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

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusSpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }
    }

    IEnumerator Attack() {
        cooldownManager.SetCooldownWithCharges(slot, _info);
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

                preFab.transform.SetParent(parent.transform, false);
                preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);


                DamageContext newContext = new(
                    _info.MinDamage,
                    _info.MaxDamage,
                    prefabInfo.PrefabDuration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>(),
                    new() {
                        { ExtraDamageContextAtributes.Penetration, (float) _info.Penetration }
                    }
                    );

                InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
                hitbox.Initialize(newContext);

                hitbox.OnHit += () => {
                    OnWeaponChange?.Invoke();
                    energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
                };
            }
            else {
                GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);

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
        End();
    }

    private void OnDestroy() {
        OnWeaponChange = null;
    }
    #endregion
}
