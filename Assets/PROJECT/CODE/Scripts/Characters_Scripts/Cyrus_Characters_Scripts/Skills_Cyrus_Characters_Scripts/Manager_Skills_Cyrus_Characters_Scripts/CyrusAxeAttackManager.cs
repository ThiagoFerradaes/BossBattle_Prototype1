using System;
using System.Collections;
using System.Collections.Generic;
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

    public static event Action OnAxeUp, OnAxeDown;

    #endregion

    #region Methods

    #region Override & Initialize
    public override void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

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

        // Bloqueando movimenta��o e outros inputs
        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        movementManager.ChangeRotationType(RotationType.MouseRotation);
        skillManager.BlockAllSkills(true);

        // Ligar animação de subir
        AnimationManager.Instance.ChangeAnimation(anim, _info.ListOfAnimationsInfo[0]);

        // VFX de subir 
        InstantiateUpAxeVFX();

        // Começar o timer
        _chargeTimeCoroutine ??= StartCoroutine(ChargeTimer());

        // Come�ar o cooldown
        cooldownManager.SetCooldownWithCharges(slot, _info);

        // Ligando o Range do prefab
        if (_info.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) SetSkillRangeIndicator(skill);

        // Checando n�vel
        if (_skillLevel == 1) healthManager.RecieveShield(_info.Level1AmountOfShield, _info.ShieldDuration);
        else if (_skillLevel > 1) healthManager.RecieveShield(_info.Level2AmountOfShield, _info.ShieldDuration);
    }

    void InstantiateUpAxeVFX() {

        OnAxeUp?.Invoke();

        if (_info.Prefabs[0].Count == 0) { Debug.Log("Nenhum VFX de subida do machado"); return; }

        SkillAnimationEvent rocksVFXEvent = _info.Prefabs[0][0];

        if (rocksVFXEvent.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(rocksVFXEvent);

    }
    public override void UseSkill(SkillSO skill) {

        // Só é chamado quando o machado ta descendo
        OnAxeDown?.Invoke();
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

        StartCoroutine(AttackCoroutine(1, 1));

    }

    protected override void FourthFunc() {
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

        AK.Wwise.Switch newSwitch = _info.ListOfSwitches[_skillLevel];
        newSwitch.SetValue(parent);
        _info.SkillSound.Post(parent);

        // On Hit
        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
            if (_skillLevel < 3) CyrusPassiveManager.Instance.AddUseSkill(slot, _info.AmountOfUsesPerLevel[_skillLevel], _info.ListOfSprites);
            if (_skillLevel == 3) InstantiateBrokenRocks();
        };
    }

    public override void InstantiateVFX(SkillAnimationEvent prefabInfo, Vector3? finalPosition = null) {

        // Esse vfx � s� do machado descendo

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetParent(parent.transform, false);
        Vector3 rotation = new(-90, -180, 90);
        preFab.transform.SetLocalPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(rotation));
        preFab.transform.SetParent(null);
        preFab.GetComponent<VFXPreFabStatic>().Initialize(prefabInfo.VFXAtribute);

    }
    void InstantiateBrokenRocks() {

        // Instanciando as pedrinhas - hit box

        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(_info.Prefabs[2][0].PreFab, TypeOfSkillPrefab.Hitbox);
        preFab.transform.localScale = _info.RocksAtributes.Size;
        preFab.transform.SetParent(parent.transform, false);
        preFab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        DamageContext newContext = new(
            _info.RocksAtributes,
            parent.GetComponent<StatusManager>()
            );

        ContinuosDamageHitBox hitbox = preFab.GetComponent<ContinuosDamageHitBox>();
        hitbox.Initialize(newContext);

        if (_info.Prefabs[2].Count <= 1 || _info.Prefabs[2][1].PrefabType != TypeOfSkillPrefab.VFX) { Debug.Log("Nenhum VFX de pedrinhas do machado"); return; }

        SkillAnimationEvent rocksVFXEvent = _info.Prefabs[2][1];

        InstantiateVFX(rocksVFXEvent);
    }
    #endregion

    #endregion
}
