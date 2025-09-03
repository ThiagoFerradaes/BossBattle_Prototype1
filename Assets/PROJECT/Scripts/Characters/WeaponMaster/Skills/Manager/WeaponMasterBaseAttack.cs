using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeaponMasterBaseAttack : SkillObjectManager {

    #region Parameters

    // Components
    WeaponMasterBaseAttackSO _info;
    WeaponManager _weaponManager;

    // Int
    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;
    Coroutine _attackCoroutine;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        
        Initialize(skill);

        if (!gameObject.activeInHierarchy) {
            gameObject.SetActive(true);
        }

        if (_timerBetweenAttacksCoroutine != null) {
            StopCoroutine(_timerBetweenAttacksCoroutine);
            _timerBetweenAttacksCoroutine = null;
        }

        _attackCoroutine ??= StartCoroutine(Attack());
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as WeaponMasterBaseAttackSO;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }

    }
    IEnumerator Attack() {
        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        // Especifico de cada ataque do combo
        string animationParameter = _attackIndex == 1 ? _info.FirstBaseAttackParameter : _info.SecondBaseAttackParameter;
        string animationName = _attackIndex == 1 ? _info.FirstBaseAttackAnimationName : _info.SecondtBaseAttackAnimationName;
        float penetration = _attackIndex == 1 ? _info.PenetrationFirstAttack : _info.PenetrationSecondAttack;
        Vector3 hitBoxPosition = _attackIndex == 1 ? _info.FirstBaseAttackHitBoxPosition : _info.SecondtBaseAttackHitBoxPosition;

        anim.SetFloat(_info.AttackSpeedAnimationParameter, attackSpeedMultiplier);

        anim.SetTrigger(animationParameter);

        AnimatorStateInfo stateInfo;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(animationName));

        _weaponManager.OnEquipRightHand(_info.SwordPrefab, _info.SwordName, _info.WeaponPosition, _info.WeaponRotation);

        int attackStateHash = stateInfo.fullPathHash;

        // Ordenando a lista de prefabs pelo tempo que eles precisam aparecer
        var prefabList = _info.Prefabs[_attackIndex];

        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < prefabList.Count; i++) {
            SkillAnimationEvent prefabInfo = prefabList[i];
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

                float attackDamageOne = _attackIndex == 1 ? _info.FirstAttackMinDamage: _info.SecondAttackMinDamage;
                float attackDamageTwo = _attackIndex == 1 ? _info.FirstAttackMaxDamage : _info.SecondAttackMaxDamage;

                InstantDamageContext newContext = new(
                    attackDamageOne,
                    attackDamageTwo,
                    prefabInfo.PrefabDuration,
                    penetration,
                    _info.HitShield,
                    _info.DamageType,
                    _info.EnemyTag,
                    parent.GetComponent<StatusManager>()
                    );

                InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
                hitbox.Initialize(newContext);

                hitbox.OnHit += () => {
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

        float cooldown = _attackIndex == 1 ? _info.CooldownBetweenAttacks : _info.Cooldown;
        float realCooldown = cooldown / attackSpeedMultiplier;

        cooldownManager.SetCooldown(slot, realCooldown);

        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        _attackIndex = _attackIndex == 1 ? _attackIndex = 2 : _attackIndex = 1;

        _weaponManager.OnDesequipRightHand();

        UnblockInputs();

        _attackCoroutine = null;

        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }

        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        gameObject.SetActive(false);
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }
    #endregion
}
