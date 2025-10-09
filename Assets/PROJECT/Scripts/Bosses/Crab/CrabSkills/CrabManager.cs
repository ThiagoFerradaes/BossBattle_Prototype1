using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum CrabArenaWall { Up, Left, Right, None}
public class CrabManager : EnemyBehaviourManager
{
    #region Parameters

    // Components
    [HideInInspector] public StatusManager StatusManager;
    [HideInInspector] public Animator Anim;
    [HideInInspector] public GameObject Player;

    [Header("Components")]
    [SerializeField] CrabWalkToTarget crabWalkToPlayerSO;
    public Transform Apicem, Vallis, SmallClaw, BigClaw;
    
    // Coroutines
    Coroutine _walkCoroutine;

    // Tide
    CrabArenaWall _currentWall = CrabArenaWall.None;

    #endregion

    #region Initialize
    public override IEnumerator Start()
    {
        // Pegando os componentes
        Player = PlayerManager.Instance.Player;
        Anim = GetComponentInChildren<Animator>();
        StatusManager = GetComponent<StatusManager>();

        return base.Start();
    }

    private void OnDestroy()
    {
        // Finalizando os Tweens
        transform.DOKill();
        Apicem.DOKill();
        Vallis.DOKill();
    }

    #endregion

    #region Walk To Player

    public void WalkToTarget(float stopDistance, Vector3 target, bool considerTolerance = true)
    {
        crabWalkToPlayerSO.WalkToTarget(this, target, stopDistance, ref _walkCoroutine, considerTolerance);
    }

    public Coroutine ReturnWalkCoroutine() => _walkCoroutine;

    public void ResetWalkCoroutine() => _walkCoroutine = null;
    #endregion

    #region Rotate

    public YieldInstruction RotateToPlayer(Transform originalPosition, float rotationSpeed)
    {
        Anim.SetBool(crabWalkToPlayerSO.WalkAnimationParameter, true);

        Vector3 playerPos = Player.transform.position;
        Vector3 playerDir = (playerPos - originalPosition.position).normalized;
        playerDir.y = 0;

        Quaternion startRot = Quaternion.LookRotation(playerDir, Vector3.up); // Angulo em quaternion do foward do inimigo até a direção do jogador

        float angle = Quaternion.Angle(transform.rotation, startRot);
        float duration = angle / rotationSpeed;

        return transform.DORotateQuaternion(startRot, duration).OnComplete(() => Anim.SetBool(crabWalkToPlayerSO.WalkAnimationParameter, false)).WaitForCompletion();

    }

    #endregion

    #region Wall

    public CrabArenaWall ReturnCurrentWall() => _currentWall;   

    public void SetCurrentArenaWall(CrabArenaWall wall) => _currentWall = wall;

    #endregion

    #region RightOrLeft

    public bool DecideIfIsRight(Vector3 target)
    {
        Vector3 crabPos = transform.position;
        target.y = crabPos.y;

        Vector3 rightSide = crabPos + transform.right * 1.1f;
        Vector3 leftSide = crabPos - transform.right * 1.1f;

        float rightDis = Vector3.Distance(target, rightSide);
        float leftDis = Vector3.Distance(target, leftSide);

        return rightDis <= leftDis;
    }

    #endregion
}
