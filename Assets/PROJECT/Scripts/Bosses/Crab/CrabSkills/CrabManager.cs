using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum CrabArenaWall { Up, Left, Right, None}
public class CrabManager : EnemyBehaviourManager
{

    [Header("Components")]
    public StatusManager StatusManager;
    [HideInInspector] public Animator Anim;

    [SerializeField] CrabWalkToPlayer crabWalkToPlayerSO;

    [HideInInspector] public GameObject Player;

    // Coroutines
    Coroutine _walkCoroutine;

    CrabArenaWall _currentWall = CrabArenaWall.None;

    #region Initialize
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

    #endregion

    #region Walk To Player

    public void WalkToTarget(float stopDistance, Vector3 target)
    {
        crabWalkToPlayerSO.WalkToTarget(this, target, stopDistance, ref _walkCoroutine);
    }

    public Coroutine ReturnWalkCoroutine() => _walkCoroutine;

    public void ResetWalkCoroutine() => _walkCoroutine = null;
    #endregion

    #region Wall

    public CrabArenaWall ReturnCurrentWall() => _currentWall;   

    public void SetCurrentArenaWall(CrabArenaWall wall) => _currentWall = wall; 

    #endregion
}
