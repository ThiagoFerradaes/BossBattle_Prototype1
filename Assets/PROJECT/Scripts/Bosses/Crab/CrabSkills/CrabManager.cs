using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class CrabManager : EnemyBehaviourManager
{

    [Header("Components")]
    public StatusManager StatusManager;
    [HideInInspector] public Animator Anim;

    [Header("Walk Animation")]
    [SerializeField] string walkAnimationName;
    [SerializeField] string walkAnimationParameter;
    [SerializeField] int animationLayer;
    [SerializeField] float rotationSpeed;
    [SerializeField] float curveMagnetude;

    [HideInInspector] public GameObject Player;

    // Coroutines
    Coroutine _walkCoroutine;

    public override IEnumerator Start()
    {

        Player = PlayerManager.Instance.Player;
        Anim = GetComponentInChildren<Animator>();

        return base.Start();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    #region Walk To Player

    public void WalkToPlayer(float distanceToPlayer)
    {
        _walkCoroutine ??= StartCoroutine(WalkToPlayerRoutine(distanceToPlayer));
    }
    public Coroutine ReturnWalkCoroutine() => _walkCoroutine;
    IEnumerator WalkToPlayerRoutine(float distanceToPlayer)
    {

        // Garantindo que esse transform não esteja fazendo nada
        transform.DOKill();

        Anim.SetBool(walkAnimationParameter, true);

        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(animationLayer);

        do
        { // Esperando animação de andar
            yield return null;
            stateInfo = Anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(walkAnimationName));

        // Rotacionando pro player
        yield return RotateToPlayer();

        // Andando até o player
        yield return PathToPlayer(distanceToPlayer);
        #endregion

        Anim.SetBool(walkAnimationParameter, false);

        _walkCoroutine = null;

    }

    public bool DecideIfIsRight()
    {
        Vector3 playerPos = Player.transform.position;
        playerPos.y = transform.position.y;

        Vector3 rightSide = transform.position + transform.right * 1.1f;
        Vector3 leftSide = transform.position - transform.right * 1.1f;

        float rightDis = Vector3.Distance(playerPos, rightSide);
        float leftDis = Vector3.Distance(playerPos, leftSide);

        bool goRight = rightDis <= leftDis;

        return goRight;
    }

    YieldInstruction RotateToPlayer()
    {
        Vector3 playerPos = Player.transform.position;
        Quaternion startRot = Quaternion.LookRotation(playerPos, Vector3.up);

        // Garante rotação só no eixo Y
        Vector3 startEuler = startRot.eulerAngles;
        startRot = Quaternion.Euler(0, startEuler.y, 0);

        float angle = Quaternion.Angle(transform.rotation, startRot);

        return transform.DORotateQuaternion(startRot, 0.2f).WaitForCompletion();
    }

    YieldInstruction PathToPlayer(float distanceToPlayer)
    {
        float curve = DecideIfIsRight() ? curveMagnetude : -curveMagnetude;

        // Decidindo o caminho
        Vector3 startPos = transform.position;
        Vector3 direction = (Player.transform.position - startPos).normalized;
        Vector3 finalPos = Player.transform.position - direction * distanceToPlayer;

        Vector3 right = Vector3.Cross(Vector3.up, direction);
        Vector3 controlPoint = startPos + direction * Vector3.Distance(startPos, finalPos) / 2f + right * curve;

        Vector3[] path = new Vector3[] { startPos, controlPoint, finalPos };

        float moveSpeed = StatusManager.ReturnStatusValue(StatusType.MoveSpeed);
        float distance = Vector3.Distance(startPos, Player.transform.position);
        float duration = distance / moveSpeed;

        return transform.DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(Player.transform).OnUpdate(() => {
            Vector3 euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }).WaitForCompletion();
    }
}
