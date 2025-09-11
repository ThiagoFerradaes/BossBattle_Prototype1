using System.Collections;
using UnityEngine;

public class BastianDashManager : SkillObjectManager {
    #region Parameters

    // Components
    BastianDashSO _info;
    Rigidbody rb;
    HealthManager _healthManager;

    #endregion

    #region Methods
    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(DashRoutine());
    }

    private void Initialize(SkillSO skill) {
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        if (_info != null) return;

        _info = skill as BastianDashSO;

        rb = parent.GetComponent<Rigidbody>();

        _healthManager = parent.GetComponent<HealthManager>();
    }

    IEnumerator DashRoutine() {

        skillManager.SkillIsInAnimation(true);

        cooldownManager.SetCooldownWithCharges(slot, _info);

        BastianPassiveManager.Instance.LooseHeat(_info.AmountOfHeatLost);

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

        skillManager.SkillIsInAnimation(false);

        End();
    }

    #endregion
}
