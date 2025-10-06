using DG.Tweening;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ Jump")]
public class CrabJumpAttack : EnemyBehaviourSO {

    // Componentes
    CrabManager _crabManager;
    Animator _anim;

    [Header("Animation")]
    [SerializeField] string preparingAnimationParameter;
    [SerializeField] string preparingAnimationName;
    [SerializeField] string jumpUpAnimationParameter;
    [SerializeField] string jumpUpAnimationName;
    [SerializeField] string landAnimationParameter;
    [SerializeField] string landDownAnimationName;
    [SerializeField] int animationLayer;

    [Header("Jump Atributes")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpDuration;
    [SerializeField] float jumpDistanceToPlayer;
    [SerializeField] float minDistanceToJump;

    [Header("Attack Atributes")]
    [SerializeField] float cooldownBetweenThisAttackAndNext;
    [SerializeField] float damage;
    [SerializeField] float jumpHitBoxSize;
    [SerializeField] float hitBoxDuration;
    [SerializeField] bool hitShield;
    [SerializeField] string jumpHitBoxName;
    [SerializeField] DamageType damageType;
    [SerializeField] List<Tags> unitsToHitTag;
    [SerializeField] GameObject jumpHitBox;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        float distanceToPlayer = Vector3.Distance(_crabManager.transform.position, _crabManager.Player.transform.transform.position);

        if (CrabArenaManager.Instance.ReturnCurrentTide() != CrabArenaState.LowTide || distanceToPlayer < minDistanceToJump) {
            _crabManager.CooldownManager.SetSkillCooldown(this);
            _crabManager.ChangeBehaviourAtRandom();
        }
        else {
            _crabManager.StartCoroutine(JumpAttack());
        }

    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;

    }

    IEnumerator JumpAttack() {

        _anim.SetTrigger(preparingAnimationParameter);
        _anim.SetBool(jumpUpAnimationParameter, true);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do { // Esperando animação de preparo do pulo
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(preparingAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        do { // Esperando o preparo do pulo terminar
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < 1);

        Vector3 finalPos = ReturnPositionCloseToPlayer();

        Sequence jumpSequence = DOTween.Sequence();

        jumpSequence.Append(_crabManager.transform.DOJump(finalPos, jumpForce, 1, jumpDuration));

        jumpSequence.Insert(jumpDuration / 2, DOVirtual.DelayedCall(0, () => _anim.SetBool(jumpUpAnimationParameter, false)));

        jumpSequence.OnComplete(() => _anim.SetTrigger(landAnimationParameter));

        yield return jumpSequence.WaitForCompletion();

        InstantiateHitBox();

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    Vector3 ReturnPositionCloseToPlayer() {
        Vector3 direction = (_crabManager.transform.position - _crabManager.Player.transform.position).normalized;
        Vector3 position = _crabManager.Player.transform.position + direction * jumpDistanceToPlayer;
        return position;
    }

    void InstantiateHitBox() {

        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(jumpHitBoxName, jumpHitBox, TypeOfSkillPrefab.Hitbox);
        prefab.transform.position = _crabManager.transform.position;
        prefab.transform.localScale = Vector3.one * jumpHitBoxSize;

        DamageContext context = new(
            damage, 
            damage, 
            hitBoxDuration, 
            hitShield,
            damageType,
            unitsToHitTag,
            _crabManager.StatusManager
            );

        InstantDamageHitBox hitBox = prefab.GetComponent<InstantDamageHitBox>();
        hitBox.Initialize(context);
    }
    
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenThisAttackAndNext);
        _crabManager.ChangeBehaviourAtRandom();

    }
}
