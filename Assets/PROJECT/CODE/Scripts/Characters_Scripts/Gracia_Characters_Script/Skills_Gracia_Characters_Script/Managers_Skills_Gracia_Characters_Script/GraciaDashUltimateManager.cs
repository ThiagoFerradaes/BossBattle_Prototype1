using System.Collections;
using UnityEngine;

public class GraciaDashUltimateManager : SkillObjectManager {
    #region Paramethers

    // Components
    GraciaDashUltimateSO _info;
    InstantDamageHitBox _principalDashDamageHitbox;

    // Int
    int _skillLevel, _playerLayer, _enemyLayer;

    // Float
    float _shieldAmount;

    // Vector3
    Vector3 _startPosition;

    // Aura
    GraciaAura _currentAura;

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

        SetParameters();

        LooseBarPoints();

        HandleEnergyManager();
    }

    void SetParameters() {
        _currentAura = GraciaPassiveManager.Instance.ReturnCurrentAura();

        _shieldAmount = healthManager.ReturnCurrentShield();

        _skillLevel = GraciaPassiveManager.Instance.ReturnCurrentSkillArea(_currentAura);

        _startPosition = parent.transform.position;
    }

    void LooseBarPoints() {
        float percentOfBarToLoose = _currentAura switch {
            GraciaAura.Blue => _info.PercentOfPassiveBarBlue,
            GraciaAura.Yellow => _info.PercentOfPassiveBarYellow,
            GraciaAura.Red => _info.PercentOfPassiveBarRed,
            GraciaAura.Green => _info.PercentOfPassiveBarGreen,
            _ => 0
        };
        float currentBarValue = GraciaPassiveManager.Instance.ReturnBarAmount(_currentAura);
        float amountToLoose = currentBarValue * percentOfBarToLoose;
        GraciaPassiveManager.Instance.ChangeBarValue(-amountToLoose, _currentAura);
    }

    void HandleEnergyManager() {
        energyManager.SetCanGainEnergy(false);

        energyManager.LooseAllEnergy();
    }

    public override void SecondFunc() {

        movementManager.ChangeIsDashing(true);

        healthManager.SetCantTakeDamage();

        _dashRoutine ??= StartCoroutine(DashRoutine());

        if(_currentAura == GraciaAura.Blue) BlueBehaviour();
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

        DecideBehaviour();

        EndWithUnblockSkills();
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

        if (_principalDashDamageHitbox != null) _principalDashDamageHitbox.ForceEnd();
    }

    #endregion

    #region Behaviours

    void DecideBehaviour() {
        switch (_currentAura) {
            case GraciaAura.Yellow: YellowBehaviour(); break;
            case GraciaAura.Green: GreenBehaviour(); break;
        }
    }
    void BlueBehaviour() {
        GameObject blueShadow = PoolingManager.Instance.ReturnPrefabFromPool(_info.BlueShadowPrefab, TypeOfSkillPrefab.Hitbox);

        blueShadow.transform.localScale = _info.BlueAtributes.Size;
        blueShadow.transform.SetPositionAndRotation(_startPosition, Quaternion.identity);

        blueShadow.GetComponent<GraciaBlueDashUltimateManager>().Initialize(_info, parent.transform, statusManager);
    }
    void YellowBehaviour() {
        energyManager.GainEnergy(_info.EnergyCost * _info.EnergyPercentToReturn);
    }
    void GreenBehaviour() {
        healthManager.Heal(_shieldAmount * _info.ShieldPercentToHeal);
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

        // Decidindo atributos
        DamageAtributes newAtributes = new(_info.Atributes);
        if (_currentAura == GraciaAura.Red) {
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate] = _info.RedCritRate;
            newAtributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage] = _info.RedCritDamage;
        }
        newAtributes.Damage *= (1 + _info.DamageIncreasePerLevel[_skillLevel]);
        DamageContext newContext = new(newAtributes, statusManager);

        _principalDashDamageHitbox = hitbox.GetComponent<InstantDamageHitBox>();
        _principalDashDamageHitbox.Initialize(newContext);
    }

    #endregion

}
