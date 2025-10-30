using DG.Tweening;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ Behaviour/ Crab/ Jump")]
public class CrabJumpAttack : EnemyBehaviourSO {

    // Componentes
    CrabManager _crabManager;
    Animator _anim;
    StunManager _stunManager;

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
    [SerializeField] DamageAtributes damageAtributes;
    [SerializeField] float jumpHitBoxSize;
    [SerializeField] GameObject jumpHitBox;

    [Header("Warning Atributes")]
    [SerializeField] float warningRepetitionAmount = 2f;
    [SerializeField] float warningDuration = 0.1f;
    [SerializeField] Vector3 warningSize;
    [SerializeField] GameObject warningPrefab;

    [Header("Stun")]
    [SerializeField] float stunDuration;

    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(JumpAttack());

    }

    public override bool MeetsCondition(EnemyBehaviourManager parent) {
        Initialize(parent);
        float distanceToPlayer = Vector3.Distance(_crabManager.transform.position, _crabManager.Player.transform.transform.position);

        bool canAttack = (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide && distanceToPlayer > minDistanceToJump);

        return canAttack;
    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _stunManager = parent.GetComponent<StunManager>();

    }

    IEnumerator JumpAttack() {
        Vector3 finalPos = ReturnPositionCloseToPlayer();

        yield return _crabManager.StartCoroutine(WarningRoutine(finalPos));

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

        Sequence jumpSequence = DOTween.Sequence();

        jumpSequence.Append(_crabManager.transform.DOJump(finalPos, jumpForce, 1, jumpDuration));

        jumpSequence.Insert(jumpDuration / 2, DOVirtual.DelayedCall(0, () => _anim.SetBool(jumpUpAnimationParameter, false)));

        jumpSequence.OnComplete(() => _anim.SetTrigger(landAnimationParameter));

        yield return jumpSequence.WaitForCompletion();

        InstantiateHitBox();

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _stunManager.StunCharacter(true);

        yield return new WaitForSeconds(stunDuration);

        _stunManager.StunCharacter(false);

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }

    IEnumerator WarningRoutine(Vector3 pos) {
        GameObject warningObject = PoolingManager.Instance.ReturnPrefabFromPool(warningPrefab, TypeOfSkillPrefab.PreCastRange);

        warningObject.transform.position = pos;
        warningObject.transform.localScale = warningSize;

        for (int i = 0; i < warningRepetitionAmount; i++) {
            warningObject.SetActive(true);
            yield return new WaitForSeconds(warningDuration / 2);
            warningObject.SetActive(false);
            yield return new WaitForSeconds(warningDuration / 2);
        }

        PoolingManager.Instance.ReturnObjectToPool(warningObject, TypeOfSkillPrefab.PreCastRange);
    }
    Vector3 ReturnPositionCloseToPlayer() {
        Vector3 direction = (_crabManager.transform.position - _crabManager.Player.transform.position).normalized;
        Vector3 position = _crabManager.Player.transform.position + direction * jumpDistanceToPlayer;
        return position;
    }

    void InstantiateHitBox() {

        GameObject prefab = PoolingManager.Instance.ReturnPrefabFromPool(jumpHitBox, TypeOfSkillPrefab.Hitbox);
        prefab.transform.position = _crabManager.transform.position;
        prefab.transform.localScale = Vector3.one * jumpHitBoxSize;

        DamageContext context = new(
            damageAtributes,
            _crabManager.StatusManager
            );

        InstantDamageHitBox hitBox = prefab.GetComponent<InstantDamageHitBox>();
        hitBox.Initialize(context);
    }

}
