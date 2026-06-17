using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillObjectManager : MonoBehaviour {
    #region Parameters
    //protected bool _preCasted;
    bool _hasStarted;

    protected PlayerSkillManager skillManager;
    protected PlayerMovementManager movementManager;
    protected GameObject parent;
    protected SkillSlot slot;
    protected Animator anim;
    protected PlayerSkillCooldownManager cooldownManager;
    protected StatusManager statusManager;
    protected EnergyManager energyManager;
    protected Coroutine animationCoroutine;
    protected HealthManager healthManager;
    protected Rigidbody rb;
    protected SkillSO info;
    Action _stopSkill;

    #endregion

    #region Methods
    public virtual void Initialize(SkillSO skill, GameObject parent, SkillSlot slot, InputAction.CallbackContext ctx) {
        if (!_hasStarted) {
            _hasStarted = true;
            skillManager = parent.GetComponent<PlayerSkillManager>();
            movementManager = parent.GetComponent<PlayerMovementManager>();
            this.parent = parent;
            anim = parent.GetComponentInChildren<Animator>();
            cooldownManager = parent.GetComponent<PlayerSkillCooldownManager>();
            statusManager = parent.GetComponent<StatusManager>();
            energyManager = parent.GetComponent<EnergyManager>();
            healthManager = parent.GetComponent<HealthManager>();
            rb = parent.GetComponent<Rigidbody>();
            info = skill;
        }

        this.slot = slot;

        HandleInput(skill, ctx);

        _stopSkill = () => CancelSkill();

        skillManager.OnStopSkills += _stopSkill;
    }


    #region Inputs
    public virtual void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {

        if (ctx.phase != InputActionPhase.Started) return;

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);

        movementManager.RotateToMouseDirection(false);

        skillManager.BlockAllSkills(true);

        UseSkill(skill);

    }

    public virtual void UnblockInputs() {

        skillManager.MoveManager.BlockWalk(false);
        skillManager.BlockAllButOneSkill(slot, false);
        skillManager.BlockAllSkills(false);
        skillManager.MoveManager.ChangeRotationType(RotationType.MoveRotation);
    }
    public virtual void UseSkill(SkillSO skill) { }
    #endregion

    #region End
    public virtual void EndWithUnblockSkills() {
        if (!skillManager.ReturnIfIsSkillAnimation()) UnblockInputs();

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Manager);

        skillManager.OnStopSkills -= _stopSkill;
    }

    public virtual void End() {
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Manager);

        skillManager.OnStopSkills -= _stopSkill;
    }

    public virtual void CancelSkill() {
        StopAllCoroutines();

        skillManager.SkillIsInAnimation(false);

        animationCoroutine = null;

        EndWithUnblockSkills();
    }
    #endregion

    #region AttackAnimation

    static readonly int attackStateHash = Animator.StringToHash("OneShotAnimation");
    protected virtual IEnumerator AttackCoroutine(int animationIndex = 0, int comboIndex = 0, float extraAnimationSpeed = 1) {
        FirstFunc();

        var animInfo = info.ListOfAnimationsInfo[animationIndex];

        AnimationManager.Instance.ChangeAnimation(anim, animInfo, extraAnimationSpeed);
        AnimationManager.Instance.BlockAnimation(true);

        yield return null;

        while (anim.IsInTransition(animInfo.AnimationLayer)) yield return null;

        var stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);

        SecondFunc();

        yield return StartCoroutine(InstantiatePrefabs(animInfo, stateInfo, comboIndex));

        ThirdFunc();

        // espera o tempo de cancel
        while (stateInfo.shortNameHash == attackStateHash && stateInfo.normalizedTime < animInfo.AnimationExitTime) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);
        }

        AnimationManager.Instance.BlockAnimation(false);

        FourthFunc();
    }

    /// <summary>
    /// Called before the animation start
    /// </summary>
    protected virtual void FirstFunc() { skillManager.SkillIsInAnimation(true); }
    /// <summary>
    /// Called after the animation start, before hitbox and vfx inistantiate
    /// </summary>
    protected virtual void SecondFunc() { }
    /// <summary>
    /// Called after hitbox and vfx instantiate
    /// </summary>
    protected virtual void ThirdFunc() { }
    /// <summary>
    /// Called after the animation ends
    /// </summary>
    protected virtual void FourthFunc() {
        // Corrotina
        animationCoroutine = null;

        // Avisando que n�o est� mais em anima��o
        skillManager.SkillIsInAnimation(false);
    }

    public virtual IEnumerator InstantiatePrefabs(AnimationInfo animInfo, AnimatorStateInfo stateInfo, int prefabIndex = 0) {

        if (info.Prefabs != null) {
            var prefabList = info.Prefabs[prefabIndex];
            prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

            for (int i = 0; i < prefabList.Count; i++) {
                var prefab = prefabList[i];

                do {
                    yield return null;
                    //Debug.Log("Esperando spawnar prefab " + prefab.PreFab.name);
                    stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);
                } while (stateInfo.shortNameHash == attackStateHash && stateInfo.normalizedTime < prefab.TimeToSpawnPreFab);

                if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab);
                else InstantiateVFX(prefab);

            }
        }
    }

    public virtual void InstantiateHitBox(SkillAnimationEvent prefab) { }
    public virtual void InstantiateVFX(SkillAnimationEvent prefab, Vector3? finalPosition = null) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);

        if (finalPosition.HasValue) {
            preFab.transform.SetPositionAndRotation(finalPosition.Value, Quaternion.identity);
        }
        else {
            preFab.transform.SetParent(parent.transform, false);
            preFab.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);
            preFab.transform.SetParent(null);
        }

        switch (prefab.VFXAtribute.VFXType) {
            case TypeOfCollider.Instant:
                preFab.GetComponent<VFXPreFabStatic>().Initialize(prefab.VFXAtribute);
                break;
            case TypeOfCollider.Continuos:
                preFab.GetComponent<VFXPreFabStatic>().Initialize(prefab.VFXAtribute);
                break;
            case TypeOfCollider.Projectile:
                preFab.GetComponent<VFXPreFabProjectile>().Initialize(prefab.VFXAtribute);
                break;
            case TypeOfCollider.Boomerang:
                preFab.GetComponent<VFXPreFabBoomerang>().Initialize(prefab.VFXAtribute, this.gameObject);
                break;
        }
    }

    #endregion
    #endregion
}
