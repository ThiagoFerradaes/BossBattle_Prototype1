using System.Collections;
using UnityEngine;

public class GraciaDashUltimateManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaDashUltimateSO _info;
    InstantDamageHitBox _principalDashDamageHitbox;

    // Int
    int _skillLevel, _playerLayer, _enemyLayer;

    // Bool
    bool _collisionForcedOn;

    // Coroutines
    Coroutine _dashRoutine;

    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill) {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.AttackAnimationParameter, _info.AttackAnimationParameter, 0));
    }

    void Initialize(SkillSO skill) {
        if (_info == null) _info = skill as GraciaDashUltimateSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

        _playerLayer = parent.layer;
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    #endregion

    #region Animation Methodes Override

    public override void FirstFunc() {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
        energyManager.LooseAllEnergy();
    }

    public override void SecondFunc() {
        DecideBehaviour();

        movementManager.ChangeIsDashing(true);

        healthManager.SetCantTakeDamage();

        _dashRoutine ??= StartCoroutine(DashRoutine());
    }

    public override void FourthFunc() {
        base.FourthFunc();

        if (_dashRoutine != null) {
            StopCoroutine(_dashRoutine);
            _dashRoutine = null;
        }

        // Se não foi forçado a ligar antes, religa no final do dash
        if (!_collisionForcedOn) {
            Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
        }

        energyManager.SetCanGainEnergy(true);

        if (_principalDashDamageHitbox != null) _principalDashDamageHitbox.ForceEnd();

        UnblockInputs();
    }

    IEnumerator DashRoutine() {

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float dashDuration = (stateInfo.length * _info.DashDuration) - (stateInfo.length * _info.TimeToStartDash);
        float remainingTime = dashDuration;

        Vector3 startPos = parent.transform.position;
        Vector3 dashDir = parent.transform.forward.normalized;
        float dashDistance = _info.DashForce * dashDuration;

        Vector3 finalPos = startPos + dashDir * dashDistance;

        Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);
        _collisionForcedOn = false;

        int dashStateHash = stateInfo.fullPathHash;

        while (stateInfo.normalizedTime < _info.TimeToStartDash) {
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.fullPathHash == dashStateHash && stateInfo.normalizedTime < _info.DashDuration) {

            rb.linearVelocity = dashDir * _info.DashForce;

            // Checa continuamente se a posição final está dentro de um inimigo
            Collider[] hits = Physics.OverlapSphere(finalPos, 0.2f, 1 << _enemyLayer);
            if (hits.Length > 0 && !_collisionForcedOn) {

                // Liga colisão de volta para parar no inimigo
                Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
                _collisionForcedOn = true;
            }

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f) break;

            stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            yield return null;
        }

        // Se não foi forçado a ligar antes, religa no final do dash
        if (!_collisionForcedOn) {
            Physics.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
        }

        movementManager.ChangeIsDashing(false);

        healthManager.SetCanTakeDamage();
    }

    #endregion

    #region Behaviours

    void DecideBehaviour() {
        Debug.Log("Decide Behaviour");
    }

    #endregion

    #region Instantiate

    public override void InstantiateHitBox(SkillAnimationEvent prefab) {

        // Pegando a hitbox na pool
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);

        // Setando o tamanho e a posição do objeto
        hitbox.transform.localScale = _info.Atributes.Size;
        hitbox.transform.SetParent(parent.transform, false);
        hitbox.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        DamageContext newContext = new(_info.Atributes, statusManager);

        _principalDashDamageHitbox = hitbox.GetComponent<InstantDamageHitBox>();
        _principalDashDamageHitbox.Initialize(newContext);
    }

    #endregion

}
