using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WalkToHighTidePosition")]
public class CrabWalkToHighTide : EnemyBehaviourSO
{

    CrabManager _crabManager;
    Animator _anim;
    StatusManager _statusManager;

    [Header("Atributes")]
    [SerializeField] float percentOfLowTide;
    [SerializeField] float highTideHeight;
    [SerializeField] float offSet;
    [SerializeField] List<Vector3> listOfPossibleFinalPositions = new();

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WalkToPosition());
    }

    public override bool MeetsCondition()
    {
        if (CrabArenaManager.Instance.ReturnCurrentTide() > CrabArenaState.LowTide && CrabArenaManager.Instance.ReturnCurrentTide() < CrabArenaState.OutgoingTide) return true;

        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide && CrabArenaManager.Instance.ReturnCurrentTidePercent() >= percentOfLowTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _statusManager = _crabManager.StatusManager;
    }

    IEnumerator WalkToPosition()
    {
        int rng = Random.Range(0, listOfPossibleFinalPositions.Count);

        Vector3 pos = listOfPossibleFinalPositions[rng];

        Vector3 dir = (pos - Vector3.zero).normalized;

        Vector3 finalPosition = pos + (dir * offSet);

        _crabManager.WalkToTarget(0, finalPosition);

        yield return _crabManager.ReturnWalkCoroutine();

        _crabManager.transform.position = new(_crabManager.transform.position.x, highTideHeight, _crabManager.transform.position.z);

        while (CrabArenaManager.Instance.ReturnCurrentTide() != CrabArenaState.HighTide) yield return null;

        pos.y = highTideHeight;

        _crabManager.WalkToTarget(0, pos);

        yield return _crabManager.ReturnWalkCoroutine();

        Debug.Log("Estou no point");
    }
}
