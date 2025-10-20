using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyrusAxeAttackManager : SkillObjectManager {

    #region Parameter
    // Components
    CyrusAxeSkillSO _info;
    WeaponManager _weaponManager;

    // Atributes
    bool _isHoldingInput;
    float _chargeTimer;
    int _skillLevel;

    // Coroutine
    Coroutine _chargeTimeCoroutine;

    #endregion

    #region Methods

    #region Override & Initialize
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

        if (Keyboard.current.ctrlKey.isPressed) return;

        Initialize(skill);

        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            _isHoldingInput = true;
            PreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            _preCasted = false;
            _isHoldingInput = false;
            ReleaseInput(skill);
        }
    }

    void Initialize(SkillSO skill) {

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info == null) _info = skill as CyrusAxeSkillSO;
        if (_weaponManager == null) _weaponManager = parent.GetComponent<WeaponManager>();

        _skillLevel = CyrusPassiveManager.Instance.ReturnSkillLevel(slot);

    }

    public override void PreCast(SkillSO skill) {

        // Bloqueando movimentação e outros inputs
        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        movementManager.ChangeRotationType(RotationType.MouseRotation);
        skillManager.BlockAllSkills(true);

        // Ligar animação
        anim.SetTrigger(_info.FirstAnimationParameterName);

        // Começar o timer
        _chargeTimeCoroutine ??= StartCoroutine(ChargeTimer());

        // Começar o cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);

        // Ligando o Range do prefab
        if (_info.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) SetSkillRangeIndicator(skill);

        // Checando nível
        if (_skillLevel == 1) healthManager.RecieveShield(_info.Level1AmountOfShield, _info.ShieldDuration);
        else if (_skillLevel > 1) healthManager.RecieveShield(_info.Level2AmountOfShield, _info.ShieldDuration);
    }

    public override void UseSkill(SkillSO skill) {
        animationCoroutine ??= StartCoroutine(Attack());
    }

    #endregion

    #region Courrotines
    IEnumerator ChargeTimer() {

        _chargeTimer = 0;

        float maxChargeTime = _skillLevel >= 2 ? _info.NewMaxChargeTime : _info.MaxChargeTime;

        _weaponManager.OnEquipRightHand(_info.WeaponPrefab, _info.WeaponName, _info.WeaponPosition, _info.WeaponRotation);

        while (_isHoldingInput || _chargeTimer < _info.MinimalChargeTime) {
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer >= maxChargeTime) break;
            yield return null; ;
        }

        if (_chargeTimer >= maxChargeTime) {
            _preCasted = false;
            ReleaseInput(_info);
        }

        _chargeTimeCoroutine = null;
    }

    IEnumerator Attack() {
        while (_chargeTimer < _info.MinimalChargeTime) yield return null;

        anim.SetTrigger(_info.SecondAnimationParameterName);

        AnimatorStateInfo stateInfo;

        do { // Esperando entrar na animação correta
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.SecondAnimationName));

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
                InstantiateHitBox(prefabInfo);
            }
            else {
                InstantiateVFX(prefabInfo);
            }
        }

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _weaponManager.OnDesequipRightHand();

        animationCoroutine = null;

        EndWithUnblockSkills();
    }

    #endregion

    #region Calculations
    float ReturnDamage() {

        float maxChargeTime = _skillLevel >= 2 ? _info.MaxChargeTime : _info.NewMaxChargeTime;

        float damage = (_chargeTimer * _info.MaxDamage) / maxChargeTime;
        return Mathf.Clamp(damage, _info.MinDamage, _info.MaxDamage);
    }

    bool ReturnBreakShield() {

        float maxChargeTime = _skillLevel >= 2 ? _info.MaxChargeTime : _info.NewMaxChargeTime;

        if (_chargeTimer >= maxChargeTime) return true;
        else return false;
    }
    #endregion

    #region Stun
    public override void CancelSkill() {
        // parar corrotinas
        if (_chargeTimeCoroutine != null) {
            StopCoroutine(_chargeTimeCoroutine);
            _chargeTimeCoroutine = null;
        }

        _preCasted = false;
        _isHoldingInput = false;

        // tirar arma
        _weaponManager?.OnDesequipRightHand();

        EndWithUnblockSkills();
    }

    #endregion

    #region Instantiate
    void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);

        DamageAtributes atributes = _info.SkillDamageAtributes;
        atributes.Damage = ReturnDamage();

        DamageContext newContext = new(
            atributes,
            prefabInfo.PrefabDuration,
            parent.GetComponent<StatusManager>(),
            new() {
                        {ExtraDamageContextAtributes.BreakShield, (bool)ReturnBreakShield() }
            }
            );

        InstantDamageHitBox hitbox = preFab.GetComponent<InstantDamageHitBox>();
        hitbox.Initialize(newContext);

        // On Hit
        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel]);
            if (_skillLevel == 3) InstantiateBrokenRocks();
        };
    }

    void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        Vector3 rotation = new Vector3(-90, -180, 90);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(rotation));
        preFab.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
    }
    void InstantiateBrokenRocks() {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(_info.BrokenRocksName,
                    _info.BrokenRocksPrefab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.localScale = Vector3.one * _info.BrokenRockSize;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DamageContext newContext = new(
            _info.RocksAtributes,
            _info.BrokenRockDuration,
            parent.GetComponent<StatusManager>(),
            new() {
             {ExtraDamageContextAtributes.DamageCooldown, _info.BrokenRockDamageCooldown }
            }
            );

        ContinuosDamageHitBox hitbox = preFab.GetComponent<ContinuosDamageHitBox>();
        hitbox.Initialize(newContext);
    }
    #endregion

    #endregion
}
