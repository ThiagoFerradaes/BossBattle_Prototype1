using DG.Tweening;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class CrabManager : EnemyBehaviourManager
{

    [Header("Components")]
    public StatusManager StatusManager;
    [HideInInspector] public Animator Anim;

    [SerializeField] CrabWalkToPlayer crabWalkToPlayerSO;

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

    public void WalkToTarget(float stopDistance, Vector3 target)
    {
        crabWalkToPlayerSO.WalkToTarget(this, target, stopDistance, ref _walkCoroutine);
    }

    public Coroutine ReturnWalkCoroutine() => _walkCoroutine;

    public void ResetWalkCoroutine() => _walkCoroutine = null;
    #endregion
}
