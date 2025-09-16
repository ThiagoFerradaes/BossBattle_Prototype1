using System.Collections;
using UnityEngine;

public class DashManager : SkillObjectManager {
    #region Parameters

    // Components
    DashSO _info;
    Rigidbody rb;
    HealthManager _healthManager;

    #endregion

    #region Methods
    public override void OnPreCast(SkillSO skill) {

        movementManager.BlockWalk(skill.BlockWalkWhilePreCasting);
        skillManager.BlockAllButOneSkill(slot, true);

        if (skill.PreCastOn && ConfigurationWhiteBoard.Instance.PreCastOn) {

            movementManager.ChangeRotationType(RotationType.MouseRotation);

            SetSkillRangeIndicator(skill);
        }

        else {

            if (ConfigurationWhiteBoard.Instance.DashToMouse) movementManager.RotateMouse(false);

            OnRelease(skill);
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

        rb = parent.GetComponent<Rigidbody>();

        _healthManager = parent.GetComponent<HealthManager>();
    }

    IEnumerator DashRoutine() {

        cooldownManager.SetCooldownWithCharges(slot, _info);

        anim.SetTrigger(_info.AnimationParameter);

        AnimatorStateInfo stateInfo;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        float elapsedTime = 0f;

        movementManager.ChangeIsDashing(true);

        _healthManager.SetCantTakeDamage();

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
        _healthManager.SetCanTakeDamage();

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        animationCoroutine = null;
        End();
    }

    public override void CancelSkill() {
        movementManager.ChangeIsDashing(false);
        _healthManager.SetCanTakeDamage();

        base.CancelSkill();
    }
    #endregion
}
