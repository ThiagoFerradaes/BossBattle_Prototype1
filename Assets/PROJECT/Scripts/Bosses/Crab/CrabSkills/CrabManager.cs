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
    [SerializeField] float rotationSpeed;

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

        Vector3 startPos = transform.position;
        Vector3 direction = (Player.transform.position - startPos).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, direction);
        float curveMagnitude = 3.5f;
        Vector3 controlPoint = startPos + direction * Vector3.Distance(startPos, Player.transform.position) / 2f + right * curveMagnitude;
        Vector3[] path = new Vector3[] { startPos, controlPoint, Player.transform.position };

        float moveSpeed = StatusManager.ReturnStatusValue(StatusType.MoveSpeed); 
        float distance = Vector3.Distance(startPos, Player.transform.position);
        float duration = distance / moveSpeed;

        yield return transform.DOPath(path, duration, PathType.CatmullRom).WaitForCompletion();

        //#region DoTween
        //// Direção do player
        //Vector3 direction = (Player.transform.position - transform.position).normalized;
        //Vector3 finalPosition = Player.transform.position - direction * distanceToPlayer;
        //finalPosition.y = transform.position.y;

        //// Velocidade e duracao
        //float moveSpeed = StatusManager.ReturnStatusValue(StatusType.MoveSpeed);
        //float duration = Vector3.Distance(transform.position, finalPosition) / moveSpeed;

        //// Rotacao
        //float rightAngle = Vector3.Angle(transform.right, direction);
        //float leftAngle = Vector3.Angle(-transform.right, direction);

        //Debug.Log($"Angles: R{rightAngle} L{leftAngle} Direction: {direction}");
        //bool useRightSide = rightAngle <= leftAngle;

        //// Agora calculamos o vetor forward que o caranguejo vai olhar
        //Vector3 forwardDir;
        //if (useRightSide) {
        //    // lado direito +90 graus
        //    forwardDir = Quaternion.Euler(0, 90, 0) * direction;
        //}
        //else {
        //    // lado esquerdo -90 graus
        //    forwardDir = Quaternion.Euler(0, -90, 0) * direction;
        //}

        //Quaternion targetRotation = Quaternion.LookRotation(forwardDir, Vector3.up);

        //// Duração da rotação: aqui você escolhe
        //float angle = Quaternion.Angle(transform.rotation, targetRotation);
        //float rotationDuration = angle / rotationSpeed;

        //// Tween de rotação
        //yield return transform.DORotateQuaternion(targetRotation, rotationDuration).WaitForCompletion();

        //#region Walk
        //// DoTween
        //Sequence walkSequence = DOTween.Sequence();
        //walkSequence.Append(transform.DOMove(finalPosition, duration));

        //yield return walkSequence.WaitForCompletion();
        //#endregion

        //direction = (Player.transform.position - transform.position).normalized;
        //targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        //angle = Quaternion.Angle(transform.rotation, targetRotation);
        //rotationDuration = angle / rotationSpeed;
        //yield return transform.DORotate(direction, rotationDuration).WaitForCompletion();

        Anim.SetBool(walkAnimationParameter, false);

        _walkCoroutine = null;
        //#endregion
    }


    #endregion
}
