using System.Collections;
using System.Resources;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyrusAxeAttackManager : SkillObjectManager {

    #region Parameter
    // Components
    CyrusAxeSkillSO _info;

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

        StartCoroutine(AttackCoroutine(0, _info.SecondAnimationParameterName, _info.SecondAnimationName, 0));
       
    }

    public override void FourthFunc() {
        base.FourthFunc();

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

        EndWithUnblockSkills();
    }

    #endregion

    #region Instantiate
    public override void InstantiateHitBox(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

        preFab.transform.localScale = _info.SkillDamageAtributes.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.identity);
        preFab.transform.SetParent(null);

        DamageAtributes atributes = new(_info.SkillDamageAtributes) {
            Damage = ReturnDamage(),
            BreakShield = ReturnBreakShield()
        };

        DamageContext newContext = new(
            atributes,
            parent.GetComponent<StatusManager>()
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

    public override void InstantiateVFX(SkillAnimationEvent prefabInfo) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        Vector3 rotation = new(-90, -180, 90);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(rotation));
        preFab.transform.SetParent(null);
        preFab.GetComponent<VFXPreFabStatic>().Initialize(prefabInfo.VFXAtribute);
    }
    void InstantiateBrokenRocks() {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(_info.BrokenRocksPrefab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.localScale = _info.RocksAtributes.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DamageContext newContext = new(
            _info.RocksAtributes,
            parent.GetComponent<StatusManager>()
            );

        ContinuosDamageHitBox hitbox = preFab.GetComponent<ContinuosDamageHitBox>();
        hitbox.Initialize(newContext);
    }
    #endregion

    #endregion
}
