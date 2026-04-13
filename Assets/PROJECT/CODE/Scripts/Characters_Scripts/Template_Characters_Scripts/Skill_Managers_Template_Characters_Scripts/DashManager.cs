using System.Collections;
using UnityEngine;

public class DashManager : SkillObjectManager {
    #region Parameters

    // Components
    DashSO _info;

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

        _info = skill as DashSO;

    }

    IEnumerator DashRoutine() {

        cooldownManager.SetCooldownWithCharges(slot, _info);

        AnimationManager.Instance.ChangeAnimation(anim, _info.ListOfAnimationsInfo[0].Animation, true, 0);

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
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < _info.TimeToStartDash);

        do {
            rb.linearVelocity = parent.transform.forward * _info.DashForce;
            elapsedTime += Time.deltaTime;

            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < _info.DashDuration);

        movementManager.ChangeIsDashing(false);
        healthManager.SetCanTakeDamage();

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        AnimationManager.Instance.ReturnToIdle(anim);
        animationCoroutine = null;
        EndWithUnblockSkills();
    }

    public override void CancelSkill() {
        movementManager.ChangeIsDashing(false);
        if (healthManager != null)
            healthManager.SetCanTakeDamage();

        base.CancelSkill();
    }
    #endregion
}
