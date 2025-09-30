using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class CrabManager : EnemyBehaviourManager {

    [Header("Components")]
    public StatusManager StatusManager;
    [HideInInspector] public Animator Anim;

    [Header("Walk Animation")]
    [SerializeField] string walkAnimationName;
    [SerializeField] string walkAnimationParameter;
    [SerializeField] int animationLayer;

    [HideInInspector] public GameObject Player;

    // Coroutines
    Coroutine _walkCoroutine;

    public override IEnumerator Start() {

        Player = PlayerManager.Instance.Player;
        Anim = GetComponentInChildren<Animator>();

        return base.Start();
    }

    #region Walk To Player

    public void WalkToPlayer(float distanceToPlayer) {
        _walkCoroutine ??= StartCoroutine(WalkToPlayerRoutine(distanceToPlayer));
    }
    public Coroutine ReturnWalkCoroutine() => _walkCoroutine;
    IEnumerator WalkToPlayerRoutine(float distanceToPlayer) {

        // Garantindo que esse transform não esteja fazendo nada
        transform.DOKill();

        #region Animation
        Anim.SetBool(walkAnimationParameter, true);

        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(animationLayer);

        do { // Esperando animação de andar
            yield return null;
            stateInfo = Anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(walkAnimationName));

        #endregion

        #region DoTween
        // Direção do player
        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Vector3 finalPosition = Player.transform.position - direction * distanceToPlayer;
        finalPosition.y = transform.position.y;

        // Velocidade e duracao
        float moveSpeed = StatusManager.ReturnStatusValue(StatusType.MoveSpeed);
        float duration = Vector3.Distance(transform.position, finalPosition) / moveSpeed;

        // DoTween
        Sequence walkSequence = DOTween.Sequence();
        walkSequence.Append(transform.DOMove(finalPosition, duration));
        walkSequence.OnComplete(() => Anim.SetBool(walkAnimationParameter, false));

        yield return walkSequence.WaitForCompletion();

        _walkCoroutine = null;
        #endregion
    }


    #endregion
}
