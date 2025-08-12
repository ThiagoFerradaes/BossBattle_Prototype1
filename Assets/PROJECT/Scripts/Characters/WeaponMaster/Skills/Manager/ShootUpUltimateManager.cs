using System;
using System.Collections;
using UnityEngine;

public class ShootUpUltimateManager : SkillObjectManager
{
    #region Parameter

    // Components
    ShootUpUltimateSO _info;
    WeaponManager _weaponManager;

    // Coroutines
    Coroutine _attackCoroutine;

    // Events
    public static event Action OnWeaponChange;

    #endregion

    #region Methods

    public override void UseSkill(SkillSO skill)
    {

        Initialize(skill);
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            _attackCoroutine ??= StartCoroutine(Attack());
        }

    }

    public override void SetSkillRangeIndicator(SkillSO skill)
    {
        currentSkillRange = SkillPoolingManager.Instance.ReturnHitboxFromPool(skill.SkillObjectRangeName, skill.SkillObjectRangeObject);
        currentSkillRange.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as ShootUpUltimateSO;

        if(_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>();
    }


    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        anim.SetTrigger(_info.AnimationParameterTrigger);

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition);
        _weaponManager.OnEquipLeftHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[0].Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < _info.Prefabs.Count; i++) {
            SkillAnimationEvent prefabInfo = _info.Prefabs[0][i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            GameObject preFab = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.PreFabName, prefabInfo.PreFab);
            preFab.transform.SetParent(parent.transform);
            preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

            if (prefabInfo.PrefabType == TypeOfSkillAnimationPrefab.Hitbox) {

                ContinuosDamageContext newContext = new(
                    _info.Damage,
                    _info.Duration,
                    _info.Penetration,
                    _info.DamageCooldown,
                    _info.HitShield,
                    _info.EnemyTag,
                    _info.DamageType,
                    parent.GetComponent<StatusManager>()
                    );

                preFab.GetComponent<ContinuosDamageHitBox>().Initialize(newContext);

                OnWeaponChange?.Invoke();
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

        UnblockInputs();
        _attackCoroutine = null;
        OnWeaponChange?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        OnWeaponChange = null;
    }
    #endregion
}
