using System;
using System.Collections;
using UnityEngine;

public class SpearAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    SpearSkillSO _info;
    WeaponManager _weaponManager;

    // Coroutines
    Coroutine _attackCoroutine;

    // Event
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

    void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as SpearSkillSO;
            cooldownManager = skillManager.CooldownManager;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }
    }

    IEnumerator Attack() {
        cooldownManager.SetCooldown(slot, _info.Cooldown);
        anim.SetTrigger(_info.SpearAttackTriggerName);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        // Ligando a arma
        _weaponManager.OnEquipRightHand(_info.SpearPrefab, _info.SpearName, _info.WeaponPosition);

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        _info.Prefabs[0].Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i <  _info.Prefabs.Count; i++) {
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

                float damage = UnityEngine.Random.Range(_info.MinDamage, _info.MaxDamage);

                InstantDamageContext newContext = new(
                    damage,
                    _info.HitBoxDuration,
                    _info.Penetration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>()
                    );

                preFab.GetComponent<InstantDamageHitBox>().Initialize(newContext);

                OnWeaponChange?.Invoke();
            }
        }

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipRightHand();
        UnblockInputs();
        _attackCoroutine = null;
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        OnWeaponChange = null;
    }
    #endregion
}
