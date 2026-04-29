using System.Collections;
using UnityEngine;

public class BastianDashManager : SkillObjectManager {
    #region Parameters

    // Components
    BastianDashSO _info;

    #endregion

    #region Methods
    public override void PreCast(SkillSO skill) {

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        skillManager.BlockAllButOneSkill(slot, true);

        if (skill.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) {

            movementManager.ChangeRotationType(RotationType.MouseRotation);

            SetSkillRangeIndicator(skill);
        }

        else {

            if (ConfigurationWhiteBoard.Instance.DashToMouse) movementManager.RotateMouse(false);

            ReleaseInput(skill);
        }
    }
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(DashRoutine());
    }

    private void Initialize(SkillSO skill) {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info != null) return;

        _info = skill as BastianDashSO;
    }

    IEnumerator DashRoutine() {

        skillManager.SkillIsInAnimation(true);

        _info.SkillSound?.Post(parent);

        cooldownManager.SetCooldownWithCharges(slot, _info);

        BastianPassiveManager.Instance.LooseHeat(_info.AmountOfHeatLost);

        AnimationManager.Instance.ChangeAnimation(anim, _info.ListOfAnimationsInfo[0]);

        yield return null;

        while (anim.IsInTransition(0)) yield return null;

        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        int attackStateHash = stateInfo.fullPathHash;

        float elapsedTime = 0f;

        movementManager.ChangeIsDashing(true);

        healthManager.SetCantTakeDamage();

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < _info.PercentOfAnimationToStartDash);

        while (elapsedTime < _info.DashDuration) {

            rb.linearVelocity = parent.transform.forward * _info.DashForce;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        movementManager.ChangeIsDashing(false);
        healthManager.SetCanTakeDamage();

        animationCoroutine = null;

        skillManager.SkillIsInAnimation(false);

        EndWithUnblockSkills();

        AnimationManager.Instance.ReturnToIdle(anim);
    }

    public override void CancelSkill() {
        movementManager.ChangeIsDashing(false);
        healthManager.SetCanTakeDamage();

        base.CancelSkill();
    }

    #endregion
}
