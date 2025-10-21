using System.Collections;
using UnityEngine;

public class CyrusBaseAttackManager : SkillObjectManager {

    #region Parameters

    // Components
    CyrusBaseAttackSO _info;
    WeaponManager _weaponManager;

    // Int
    int _attackIndex = 1;

    // Coroutine
    Coroutine _timerBetweenAttacksCoroutine;

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

        animationCoroutine ??= StartCoroutine(Attack());
    }

    private void Initialize(SkillSO skill) {
        if (_info == null) {
            _info = skill as CyrusBaseAttackSO;
            _weaponManager = parent.GetComponent<WeaponManager>();
        }

    }
    IEnumerator Attack() {
        float attackSpeedMultiplier = GetAttackSpeedMultiplier();

        skillManager.SkillIsInAnimation(true);

        // Especifico de cada ataque do combo
        string animationParameter = _attackIndex == 1 ? _info.FirstBaseAttackParameter : _info.SecondBaseAttackParameter;
        string animationName = _attackIndex == 1 ? _info.FirstBaseAttackAnimationName : _info.SecondtBaseAttackAnimationName;
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


            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefabInfo);
            else InstantiateVFX(prefabInfo);
        }

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        FinishAttack(attackSpeedMultiplier);
    }

    void FinishAttack(float attackSpeedMultiplier) {
        float cooldown = _attackIndex == 1 ? _info.CooldownBetweenAttacks : _info.Cooldown;
        float realCooldown = cooldown / attackSpeedMultiplier;

        cooldownManager.SetCooldownSingleCharge(slot, realCooldown);

        anim.SetFloat(_info.AttackSpeedAnimationParameter, 1);

        _attackIndex = _attackIndex == 1 ? _attackIndex = 2 : _attackIndex = 1;

        _weaponManager.OnDesequipRightHand();

        UnblockInputs();

        animationCoroutine = null;

        _timerBetweenAttacksCoroutine ??= StartCoroutine(CooldownBetweenAttacks());

        skillManager.SkillIsInAnimation(false);
    }

    IEnumerator CooldownBetweenAttacks() {
        float timer = 0;

        while (timer <= _info.MaxTimeBetweenAttacks) {
            timer += Time.deltaTime;
            yield return null;
        }

        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;
        End();
    }

    public override void CancelSkill() {
        _attackIndex = 1;
        _timerBetweenAttacksCoroutine = null;

        base.CancelSkill();
    }

    float GetAttackSpeedMultiplier() {
        float baseSpeed = statusManager.ReturnStatusValue(StatusType.AttackSpeed);
        return Mathf.Max(0.1f, baseSpeed);
    }

    void InstantiateHitBox(SkillAnimationEvent prefabInfo) {

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
            prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        DamageAtributes atributes = _attackIndex == 1 ? _info.FirstAttackAtributes : _info.SecondAttackAtributes;

        DamageContext newContext = new(
            atributes,
            prefabInfo.PrefabDuration,
            parent.GetComponent<StatusManager>()
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
        };
    }

    void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
    }
    #endregion
}
