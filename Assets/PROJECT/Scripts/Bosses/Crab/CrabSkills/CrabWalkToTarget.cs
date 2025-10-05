using DG.Tweening;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WalkToPlayer")]
public class CrabWalkToTarget : ScriptableObject
{
    CrabManager _crabManager;

    [Header("Walk Animation")]
    public string WalkAnimationParameter;
    public float RotationSpeed;
    [SerializeField] string walkAnimationName;
    [SerializeField] int animationLayer;
    [SerializeField] float curveMagnetude;
    [SerializeField] float walkTolerance;

    public void WalkToTarget(CrabManager manager, Vector3 target, float distanceToTarget, ref Coroutine walkRoutine)
    {
        if (_crabManager == null) _crabManager = manager;

        float currentDistanceToTarget = Vector3.Distance(_crabManager.transform.position, target);
        if (currentDistanceToTarget <= distanceToTarget + walkTolerance) return;

        walkRoutine ??= _crabManager.StartCoroutine(WalkToTargetRoutine(distanceToTarget, target));
    }
    IEnumerator WalkToTargetRoutine(float stopDistance, Vector3 target)
    {

        // Garantindo que esse transform não esteja fazendo nada
        _crabManager.transform.DOKill();

        _crabManager.Anim.SetBool(WalkAnimationParameter, true);

        AnimatorStateInfo stateInfo = _crabManager.Anim.GetCurrentAnimatorStateInfo(animationLayer);

        do
        { // Esperando animação de andar
            yield return null;
            stateInfo = _crabManager.Anim.GetCurrentAnimatorStateInfo(animationLayer);
        } while (!stateInfo.IsName(walkAnimationName));

        // Rotacionando pro player
        yield return RotateToTarget();

        // Andando até o player
        yield return PathToTarget(stopDistance, target);

        _crabManager.Anim.SetBool(WalkAnimationParameter, false);

        _crabManager.ResetWalkCoroutine();

    }

    YieldInstruction RotateToTarget()
    {
        Vector3 playerPos = _crabManager.Player.transform.position;
        Quaternion startRot = Quaternion.LookRotation(playerPos, Vector3.up);

        // Garante rotação só no eixo Y
        Vector3 startEuler = startRot.eulerAngles;
        startRot = Quaternion.Euler(0, startEuler.y, 0);

        return _crabManager.transform.DORotateQuaternion(startRot, 0.2f).WaitForCompletion();
    }

    YieldInstruction PathToTarget(float stopDistance, Vector3 target)
    {
        float curve = _crabManager.DecideIfIsRight(target) ? curveMagnetude : -curveMagnetude;

        // Decidindo o caminho
        Vector3 startPos = _crabManager.transform.position;
        Vector3 direction = (target - startPos).normalized;
        Vector3 finalPos = target - direction * stopDistance;

        Vector3 right = Vector3.Cross(Vector3.up, direction);
        Vector3 controlPoint = startPos + direction * Vector3.Distance(startPos, finalPos) / 2f + right * curve;

        Vector3[] path = new Vector3[] { startPos, controlPoint, finalPos };

        float moveSpeed = _crabManager.StatusManager.ReturnStatusValue(StatusType.MoveSpeed);
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / moveSpeed;

        return _crabManager.transform.DOPath(path, duration, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(_crabManager.Player.transform).OnUpdate(() => {
            Vector3 euler = _crabManager.transform.rotation.eulerAngles;
            _crabManager.transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }).WaitForCompletion();
    }

}
