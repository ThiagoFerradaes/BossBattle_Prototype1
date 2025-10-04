using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaterMoon")]
public class CrabWaterMoonAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    Transform _vallis;

    [Header("Animation")]
    [SerializeField] string animationParameter;
    [SerializeField] string animationName;
    [SerializeField] int animationLayer;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Attack Atributes")]
    [SerializeField] float attackAngleAmplitude;
    [SerializeField] float cooldownBetweenShoots;
    [SerializeField] float cooldownRotations;
    [SerializeField] float rootationSpeed;
    [SerializeField] float durationOfRotationToRight;
    [SerializeField] float cooldownBetweenAttacks;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Debug.Log("Water Moon");

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
        Quaternion centerAngle = Quaternion.Euler(0, 0, 0);
        Quaternion rightAngle = Quaternion.Euler(0, attackAngleAmplitude / 2, 0);
        Quaternion leftAngle = Quaternion.Euler(0, -attackAngleAmplitude / 2, 0);

        yield return _vallis.DOLocalRotateQuaternion(rightAngle, durationOfRotationToRight).WaitForCompletion();

        float shootDuration = Quaternion.Angle(rightAngle, leftAngle) / rootationSpeed;

        yield return RotateAndShoot(leftAngle, shootDuration);

        yield return new WaitForSeconds(cooldownRotations);

        yield return RotateAndShoot(rightAngle, shootDuration);

        yield return _vallis.DOLocalRotateQuaternion(centerAngle, durationOfRotationToRight).WaitForCompletion();

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator RotateAndShoot(Quaternion endAngle, float duration)
    {
        Tween rotateTween = _vallis.DOLocalRotateQuaternion(endAngle, duration);
        float timer = 0f;

        while (rotateTween.IsActive() && !rotateTween.IsComplete())
        {
            timer += Time.deltaTime;

            if (timer > cooldownBetweenShoots)
            {
                timer = 0f;

                yield return _crabManager.StartCoroutine(Shoot());
            }

            yield return null;
        }
    }

    IEnumerator Shoot()
    {
        _anim.SetTrigger(animationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do
        { // Aguardando entrar na animação
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(animationName));

        int attackStateHash = stateInfo.fullPathHash;

        if (prefabs != null)
        {
            var prefabList = prefabs;
            prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

            for (int i = 0; i < prefabList.Count; i++)
            {
                var prefab = prefabList[i];

                do
                {
                    yield return null;
                    stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
                } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < prefab.TimeToSpawnPreFab);

                if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab);
                else InstantiateVFX(prefab);

            }
        }

        do
        {
            yield return null;
            stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < 1);
    }

    void InstantiateHitBox(SkillAnimationEvent prefab)
    {

    }

    void InstantiateVFX(SkillAnimationEvent prefab) { }

    IEnumerator CooldownBetweenAttacks()
    {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(1);
    }
}
