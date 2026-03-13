using System;
using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class AnimationInfo {
    public AnimationClip Animation;
    public int AnimationLayer;
    public float AnimationSpeed = 1;
    [Range(0,1)] public float AnimationExitTime = 1;
    public bool IsAnimationTrigger = true;
}
public abstract class SkillObjectManager : MonoBehaviour {
    #region Parameters
    protected bool _preCasted;
    bool _hasStarted;

    protected PlayerSkillManager skillManager;
    protected PlayerMovementManager movementManager;
    protected GameObject parent;
    protected GameObject currentSkillRange;
    protected SkillSlot slot;
    protected Animator anim;
    protected PlayerSkillCooldownManager cooldownManager;
    protected StatusManager statusManager;
    protected EnergyManager energyManager;
    protected Coroutine animationCoroutine;
    protected HealthManager healthManager;
    protected Rigidbody rb;
    protected SkillSO info;

    AnimatorOverrideController overrideController;
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

            overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
            anim.runtimeAnimatorController = overrideController;
        }
        this.slot = slot;
        HandleInput(skill, ctx);

        _stopSkill = () => CancelSkill();

        skillManager.OnStopSkills += _stopSkill;
    }

    #region Inputs
    public virtual void HandleInput(SkillSO skill, InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Started) {
            _preCasted = true;
            PreCast(skill);
        }
        if (ctx.phase == InputActionPhase.Canceled && _preCasted) {
            ReleaseInput(skill);
        }
    }
    public virtual void PreCast(SkillSO skill) {

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        skillManager.BlockAllButOneSkill(slot, true);

        if (skill.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) {

            movementManager.ChangeRotationType(RotationType.MouseRotation);

            SetSkillRangeIndicator(skill);
        }

        else {

            movementManager.RotateMouse(false);

            ReleaseInput(skill);
        }
    }

    public virtual void ReleaseInput(SkillSO skill) {

        _preCasted = false;
        ReleaseSkillRangeIndicator();
        skillManager.BlockAllSkills(true);
        movementManager.ChangeRotationType(RotationType.MoveRotation);

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

    #region RangeIndicator
    public virtual void SetSkillRangeIndicator(SkillSO skill) {
        currentSkillRange = PoolingManager.Instance.ReturnPrefabFromPool(skill.SkillObjectRangeObject, TypeOfSkillPrefab.PreCastRange);

        currentSkillRange.transform.SetParent(parent.transform);

        float groundY = ArenaManager.Instance.FindGroundHeight(parent.transform.position);
        Vector3 groundPos = new(0, groundY - parent.transform.position.y, 0);

        currentSkillRange.transform.SetLocalPositionAndRotation(groundPos, Quaternion.identity);

        currentSkillRange.SetActive(true);
    }

    void ReleaseSkillRangeIndicator() {
        if (currentSkillRange == null) return;

        PoolingManager.Instance.ReturnObjectToPool(currentSkillRange, TypeOfSkillPrefab.PreCastRange);
        currentSkillRange = null;

    }
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
        ReleaseSkillRangeIndicator();

        _preCasted = false;
        skillManager.SkillIsInAnimation(false);

        animationCoroutine = null;

        EndWithUnblockSkills();
    }
    #endregion

    #region AttackAnimation
    static readonly int attackStateHash = Animator.StringToHash("Attack");
    public virtual IEnumerator AttackCoroutine(int animationIndex = 0, int comboIndex = 0) {
        FirstFunc();

        AnimationInfo animInfo = info.ListOfAnimationsInfo[animationIndex];

        overrideController["Bastian_BasicAttack_1"] = animInfo.Animation;

        anim.CrossFade("Attack", 0.05f, animInfo.AnimationLayer, 0f);

        yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);

        SecondFunc();

        yield return StartCoroutine(InstantiatePrefabs(animInfo, stateInfo, comboIndex));

        ThirdFunc();

        // espera o tempo de cancel
        while (stateInfo.shortNameHash == attackStateHash &&
               stateInfo.normalizedTime < animInfo.AnimationExitTime) {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);
        }

        FourthFunc();
    }

    /// <summary>
    /// Called before the animation start
    /// </summary>
    public virtual void FirstFunc() { skillManager.SkillIsInAnimation(true); }
    /// <summary>
    /// Called after the animation start, before hitbox and vfx inistantiate
    /// </summary>
    public virtual void SecondFunc() { }
    /// <summary>
    /// Called after hitbox and vfx instantiate
    /// </summary>
    public virtual void ThirdFunc() { }
    /// <summary>
    /// Called after the animation ends
    /// </summary>
    public virtual void FourthFunc() {
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
                    stateInfo = anim.GetCurrentAnimatorStateInfo(animInfo.AnimationLayer);
                } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < prefab.TimeToSpawnPreFab);

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
