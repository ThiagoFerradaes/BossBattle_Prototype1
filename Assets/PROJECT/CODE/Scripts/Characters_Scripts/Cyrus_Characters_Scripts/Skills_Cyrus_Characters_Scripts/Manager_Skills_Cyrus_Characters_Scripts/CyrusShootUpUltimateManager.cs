using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyrusShootUpUltimateManager : SkillObjectManager {
    #region Parameter

    // Components
    CyrusShootUpSO _info;
    WeaponManager _weaponManager;
    EnergyManager _energyManager;

    // Atributes
    int _skillLevel = 0;
    float _amountOfHits;

    // Coroutines
    Coroutine _durationCoroutine, _damageCoroutine;

    #endregion

    #region Methods

    #region Override & Initialize
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

    public override void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = PoolingManager.Instance.ReturnPrefabFromPool(skill.SkillObjectRangeName,
            skill.SkillObjectRangeObject, TypeOfSkillPrefab.PreCastRange);

        currentSkillRange.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusShootUpSO;

        if (_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>(); 
        if (_energyManager == null) _energyManager = parent.GetComponent<EnergyManager>();

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);
    }

    #endregion

    #region Coroutines
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
                _damageCoroutine ??= StartCoroutine(Damage(prefabInfo));
                _durationCoroutine ??= StartCoroutine(Duration());
                if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
            }

            else {
                InstantiateVFX(prefabInfo);
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

        UnblockInputs();
    }
    IEnumerator Duration() {
        float duration = _skillLevel > 0 ? _info.Level1Duration : _info.Duration;
        yield return new WaitForSeconds(duration);

        EndWithUnblockSkills();
    }
    IEnumerator Damage(SkillAnimationEvent prefabInfo) {
        float damageCooldown = _skillLevel == 3 ? _info.Level3DamageCooldown : _info.DamageCooldown;

        while (true) {
            yield return new WaitForSeconds(damageCooldown);
            InstantiateHitBox(prefabInfo);
        }
    }

    #endregion

    #region Instantiate
    void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.localScale = Vector3.one * _info.Size;
        preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        float charCritRate = statusManager.ReturnStatusValue(StatusType.CritRate);
        float critRate = _skillLevel > 1? charCritRate + _info.AditionalCritRate: charCritRate; 

        float charCritDamage = statusManager.ReturnStatusValue(StatusType.CritDamage);
        float critDamage = _skillLevel > 2? charCritDamage + (_amountOfHits * _info.AditionalCritDamagePerHit) : charCritDamage;

        DamageContext newContext = new(
            _info.Atributes,
            prefabInfo.PrefabDuration,
            parent.GetComponent<StatusManager>(),
            new() {
                {ExtraDamageContextAtributes.CritRate, critRate},
                {ExtraDamageContextAtributes.CritDamage, critDamage}
            }
            );
        InstantDamageHitBox hitBox = preFab.GetComponent<InstantDamageHitBox>();

        hitBox.Initialize(newContext);

        hitBox.OnHit += () => {
            _amountOfHits++;
        };

        
    }

    void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        float duration = _skillLevel > 0 ? _info.Level1Duration : _info.Duration;

        preFab.GetComponent<VFXPreFab>().Initialize(duration);
    }
    #endregion

    #region End
    public override void EndWithUnblockSkills() {

        _durationCoroutine = null;
        StopCoroutine(_damageCoroutine);
        _damageCoroutine = null;    

        _amountOfHits = 0;

        base.End();
    }
    #endregion

    #endregion
}
