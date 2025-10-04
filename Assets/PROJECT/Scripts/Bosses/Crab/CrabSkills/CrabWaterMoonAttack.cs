using DG.Tweening;
using System.Collections;
using System.Drawing;
using UnityEngine;

public class CrabWaterMoonAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    Transform _vallis;

    [Header("Animation")]
    [SerializeField] string animationParameter;

    [Header("Attack Atributes")]
    [SerializeField] float attackAngleAmplitude;
    [SerializeField] float cooldownBetweenShoots;
    [SerializeField] float rootationSpeed;
    [SerializeField] float durationOfRotationToRight;
    [SerializeField] float cooldownBetweenAttacks;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WaterMoonAttack());
    }

    public override bool MeetsCondition()
    {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _vallis = _crabManager.Vallis;
    }

    IEnumerator WaterMoonAttack()
    {
        Quaternion centerAngle = Quaternion.Euler(0,0,0);
        Quaternion rightAngle = Quaternion.Euler(0, attackAngleAmplitude / 2, 0);
        Quaternion leftAngle = Quaternion.Euler(0, -attackAngleAmplitude / 2, 0);

        yield return _vallis.DOLocalRotateQuaternion(rightAngle, durationOfRotationToRight).WaitForCompletion();

        float shootDuration = Quaternion.Angle(rightAngle, leftAngle) / rootationSpeed;

        yield return RotateAndShoot(leftAngle, shootDuration);
        yield return RotateAndShoot(rightAngle, shootDuration);

        yield return _vallis.DOLocalRotateQuaternion(centerAngle, durationOfRotationToRight).WaitForCompletion();

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    YieldInstruction RotateAndShoot(Quaternion endAngle, float duration)
    {
        float timer = 0f;

        return _vallis.DOLocalRotateQuaternion(endAngle, duration).OnUpdate(() =>
        {
            timer += Time.deltaTime;

            if (timer > cooldownBetweenShoots)
            {
                _anim.SetTrigger(animationParameter);
                timer = 0f;
                InstantiateProjectile();
            }

        }).WaitForCompletion();
    }
    void InstantiateProjectile()
    {

    }

    IEnumerator CooldownBetweenAttacks()
    {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(1);
    }
}
