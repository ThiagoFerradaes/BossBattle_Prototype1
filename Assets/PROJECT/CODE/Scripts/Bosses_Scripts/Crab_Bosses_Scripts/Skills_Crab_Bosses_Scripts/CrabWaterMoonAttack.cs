using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaterMoon")]
public class CrabWaterMoonAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    Transform _vallis;
    StatusManager _statusManager;

    [Header("Animation")]
    [SerializeField] string animationParameter;
    [SerializeField] string animationName;
    [SerializeField] int animationLayer;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Rotation Atributes")]
    [SerializeField] float attackAngleAmplitude;
    [SerializeField] float cooldownBetweenShoots;
    [SerializeField] float cooldownRotations;
    [SerializeField] float rootationSpeed;
    [SerializeField] float durationOfRotationToRight;

    [Header("Attack Atributes")]
    [SerializeField] float projectileSize;
    [SerializeField] DamageAtributes damageAtributes;

    Coroutine _attackCoroutine, _rotateAndShootCoroutine;

    Action<CrabArenaState> _onTideChange;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _attackCoroutine ??= _crabManager.StartCoroutine(WaterMoonAttack());

        CrabArenaManager.Instance.OnEndTide += _onTideChange;
    }

    void OnChangeTide(CrabArenaState tide)
    {
        if (tide != CrabArenaState.HighTide) return;

        if (_attackCoroutine != null)
        {
            _crabManager.StopCoroutine(_attackCoroutine);

            _crabManager.CooldownManager.SetSkillCooldown(this);

            _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());

            _attackCoroutine = null;

            _vallis.rotation = Quaternion.Euler(0, 0, 0);   
        }

        if (_rotateAndShootCoroutine != null)
        {
            _crabManager.StopCoroutine(_rotateAndShootCoroutine);
            _anim.SetBool(animationParameter, false);
        }
    }
    public override bool MeetsCondition()
    {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _onTideChange = OnChangeTide;
        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _vallis = _crabManager.Vallis;
        _statusManager = _crabManager.StatusManager;
    }

    IEnumerator WaterMoonAttack()
    {
        //Vector3 playerDir = (_crabManager.Player.transform.position - _vallis.transform.position).normalized;
        //playerDir.x = 0f;
        //playerDir.z = 0f;
        //Quaternion playerAngle = Quaternion.LookRotation(playerDir, Vector3.up);
        //Debug.Log($" quatarion : {playerAngle} and vector {playerAngle.eulerAngles}");

        Quaternion centerAngle = Quaternion.Euler(0, 0, 0);

        Quaternion rightAngle = Quaternion.Euler(0, attackAngleAmplitude / 2f, 0);
        Quaternion leftAngle = Quaternion.Euler(0, -attackAngleAmplitude / 2f, 0);


        yield return _vallis.DOLocalRotateQuaternion(rightAngle, durationOfRotationToRight).WaitForCompletion();

        float shootDuration = Quaternion.Angle(rightAngle, leftAngle) / rootationSpeed;

        yield return _rotateAndShootCoroutine ??= _crabManager.StartCoroutine(RotateAndShoot(leftAngle, shootDuration));

        yield return new WaitForSeconds(cooldownRotations);

        yield return _rotateAndShootCoroutine ??= _crabManager.StartCoroutine(RotateAndShoot(rightAngle, shootDuration));

        yield return _vallis.DOLocalRotateQuaternion(centerAngle, durationOfRotationToRight).WaitForCompletion();

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());

        _attackCoroutine = null;
    }

    IEnumerator RotateAndShoot(Quaternion endAngle, float duration)
    {
        _anim.SetBool(animationParameter, true);

        var prefabList = prefabs;
        prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do
        { // Aguardando entrar na animação
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(animationName));

        float timer = 0f;

        yield return _vallis.DOLocalRotateQuaternion(endAngle, duration).SetEase(Ease.Linear).OnUpdate(() =>
        {
            timer += Time.deltaTime;

            if (timer >= cooldownBetweenShoots)
            {

                timer -= cooldownBetweenShoots;

                Shoot(prefabList);
            }
        }).WaitForCompletion();

        _anim.SetBool(animationParameter, false);
    }

    void Shoot(List<SkillAnimationEvent> prefabList)
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            var prefab = prefabList[i];

            if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab);
            else InstantiateVFX(prefab);

        }
    }
    void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = Vector3.one * projectileSize;
        Vector3 pos = _vallis.position;
        pos += prefab.PreFabPosition;
        hitbox.transform.position = pos;
        hitbox.transform.rotation = _vallis.rotation;

        DamageContext context = new(
            damageAtributes,
            _statusManager
            );

        ProjectileDamageHitBox projectileDamageHitBox = hitbox.GetComponent<ProjectileDamageHitBox>();
        projectileDamageHitBox.Initialize(context);
    }

    void InstantiateVFX(SkillAnimationEvent prefab)
    {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }
}
