using System.Collections;
using UnityEngine;

public class DashManager : SkillObjectManager {
    #region Parameters

    // Components
    DashSO _info;
    Rigidbody rb;

    // Coroutine
    Coroutine _dashCoroutine;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        _dashCoroutine ??= StartCoroutine(DashRoutine());
    }

    private void Initialize(SkillSO skill) {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info != null) return;

        _info = skill as DashSO;

        rb = parent.GetComponent<Rigidbody>();
    }

    IEnumerator DashRoutine() {

        cooldownManager.SetCooldown(slot, _info.Cooldown);

        anim.SetTrigger(_info.AnimationParameter);

        AnimatorStateInfo stateInfo;

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(_info.AnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        float elapsedTime = 0f;

        movementManager.ChangeIsDashing(true);

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

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        _dashCoroutine = null;
        gameObject.SetActive(false);
        UnblockInputs();
    }

    #endregion
}
