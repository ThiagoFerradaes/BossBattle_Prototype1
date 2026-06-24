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

    // Coroutine
    Coroutine _chargeTimeCoroutine;

    public static event Action<GameObject> OnAxeUp, OnAxeDown;

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
        if (BattleRankManager.Instance.ReturnCurrentRank() == BattleRank.SS) healthManager.RecieveShield(_info.AmountOfShield, _info.ShieldDuration);
    }

    void InstantiateUpAxeVFX() {


        if (_info.Prefabs[0].Count == 0) { Debug.Log("Nenhum VFX de subida do machado"); return; }

        SkillAnimationEvent rocksVFXEvent = _info.Prefabs[0][0];

        if (rocksVFXEvent.PrefabType == TypeOfSkillPrefab.VFX) InstantiateVFX(rocksVFXEvent);

        OnAxeUp?.Invoke(parent);
    }
    public override void UseSkill(SkillSO skill) {

        // Só é chamado quando o machado ta descendo
        OnAxeDown?.Invoke(parent);
        animationCoroutine ??= StartCoroutine(Attack());
    }

    #endregion

    #region Courrotines
    IEnumerator ChargeTimer() {

        _chargeTimer = 0;

        float maxChargeTime = _info.MaxChargeTime;

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

        float damage = (_chargeTimer * _info.MaxDamage) / _info.MaxChargeTime;
        return Mathf.Clamp(damage, _info.MinDamage, _info.MaxDamage);
    }

    bool ReturnBreakShield() {

        if (_chargeTimer >= _info.MaxChargeTime) return true;
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

        //AK.Wwise.Switch newSwitch = _info.ListOfSwitches[_skillLevel];
        //newSwitch.SetValue(parent);
        //_info.SkillSound.Post(parent);

        // On Hit
        hitbox.OnHit += () => {
            energyManager.GainEnergy(_info.FlatEnergyGainPerHit);
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

    #endregion

    #endregion
}
