using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyrusShootUpUltimateManager : SkillObjectManager {
    #region Parameter

    // Components
    CyrusShootUpSO _info;

    // Atributes
    int _skillLevel = 0;
    float _amountOfHits;

    // Coroutines
    Coroutine _durationCoroutine, _damageCoroutine;

    #endregion

    #region Methods

    #region Override & Initialize
    public override void UseSkill(SkillSO skill) {

        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AnimationParameterTrigger, _info.AnimationName, 0));

    }

    public override void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = PoolingManager.Instance.ReturnPrefabFromPool(skill.SkillObjectRangeObject, TypeOfSkillPrefab.PreCastRange);

        currentSkillRange.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentSkillRange.SetActive(true);
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as CyrusShootUpSO;

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    #endregion

    #region Coroutines

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.LooseAllEnergy();
        energyManager.SetCanGainEnergy(false);
    }

    public override void FourthFunc() {
        base.FourthFunc();

        energyManager.SetCanGainEnergy(true);

        UnblockInputs();
    }

    public override IEnumerator InstantiatePrefabs(int attackStateHash, AnimatorStateInfo stateInfo, int prefabIndex = 0) {
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
    }

    IEnumerator Duration() {
        float duration = _skillLevel > 0 ? _info.Level1Duration : _info.Duration;

        yield return new WaitForSeconds(duration);

        if (_damageCoroutine != null) {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }

        End();
    }
    IEnumerator Damage(SkillAnimationEvent prefabInfo) {
        float damageCooldown = _skillLevel == 3 ? _info.Level3DamageCooldown : _info.Atributes.DamageCooldown;

        while (true) {
            InstantiateHitBox(prefabInfo);
            yield return new WaitForSeconds(damageCooldown);
        }
    }

    #endregion

    #region Instantiate
    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.localScale = _info.Atributes.Size;
        preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        float charCritDamage = statusManager.ReturnStatusValue(StatusType.CritDamage);
        float critDamage = charCritDamage + (_amountOfHits * _info.AditionalCritDamagePerHit);

        DamageAtributes atributes = new(_info.Atributes);
        if(_skillLevel > 1) atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.AditionalCritRate;
        if (_skillLevel > 2) atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = critDamage;

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
            );
        InstantDamageHitBox hitBox = preFab.GetComponent<InstantDamageHitBox>();

        hitBox.Initialize(newContext);

        hitBox.OnHit += () => {
            _amountOfHits++;
        };


    }

    public override void InstantiateVFX(SkillAnimationEvent prefabInfo, Vector3? finalPosition = null) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        float duration = _skillLevel > 0 ? _info.Level1Duration : _info.Duration;

        preFab.GetComponent<VFXPreFabStatic>().Initialize(duration);
    }
    #endregion

    #region End

    public override void End() {

        if (_durationCoroutine != null) {
            StopCoroutine(_durationCoroutine);
            _durationCoroutine = null;
        }

        if (_damageCoroutine != null) {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }
        _amountOfHits = 0;

        base.End();
    }
    #endregion

    #endregion
}
