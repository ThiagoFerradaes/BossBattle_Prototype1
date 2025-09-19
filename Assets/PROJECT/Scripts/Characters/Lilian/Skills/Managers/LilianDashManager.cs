using System.Collections;
using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public class LilianDashManager : SkillObjectManager
{
    #region Parameters

    // Components
    DashSO _info;
    Rigidbody rb;
    HealthManager _healthManager;

    // Atributes
    int _playerLayer, _enemyLayer;

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

        _playerLayer = parent.layer;
        _enemyLayer = LayerMask.NameToLayer("Enemy");
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

        movementManager.ChangeIsDashing(true);

        _healthManager.SetCantTakeDamage();

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < _info.TimeToStartDash);

        // Dash
        Coroutine dashRoutine = StartCoroutine(InDashRoutine(attackStateHash));
        yield return dashRoutine;

        movementManager.ChangeIsDashing(false);
        _healthManager.SetCanTakeDamage();

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        animationCoroutine = null;
        End();
    }

    IEnumerator InDashRoutine(int dashStateHash) {

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float dashDuration = (stateInfo.length * _info.DashDuration) - (stateInfo.length * _info.TimeToStartDash);
        float remainingTime = dashDuration;

        Vector3 startPos = parent.transform.position;
        Vector3 dashDir = parent.transform.forward.normalized;
        float dashDistance = _info.DashForce * dashDuration;

        Vector3 finalPos = startPos + dashDir * dashDistance;

        // Por padrão, desliga colisão
        Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);
        bool collisionForcedOn = false;

        while (stateInfo.fullPathHash == dashStateHash && stateInfo.normalizedTime < _info.DashDuration) {
            // Movimento do dash
            rb.linearVelocity = dashDir * _info.DashForce;

            // Checa continuamente se a posição final está dentro de um inimigo
            Collider[] hits = Physics.OverlapSphere(finalPos, 0.2f, 1 << _enemyLayer);
            if (hits.Length > 0 && !collisionForcedOn) {
               
                // Liga colisão de volta para parar no inimigo
                Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
                collisionForcedOn = true;
            }

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f) break;

            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        // Se não foi forçado a ligar antes, religa no final do dash
        if (!collisionForcedOn) {
            Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
        }
    }
    public override void CancelSkill() {
        movementManager.ChangeIsDashing(false);
        Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
        if (_healthManager != null)
            _healthManager.SetCanTakeDamage();

        base.CancelSkill();
    }
    #endregion
}
