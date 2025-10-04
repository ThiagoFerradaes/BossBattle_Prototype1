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
    [SerializeField] float cooldownBetweenAttacks;

    [Header("Attack Atributes")]
    [SerializeField] float projectileSize;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileDistance;
    [SerializeField] float damage;
    [SerializeField] bool hitShield;
    [SerializeField] List<Tags> unitsToHit;
    [SerializeField] DamageType damageType;

    Coroutine _shootRoutine;


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
        _statusManager = _crabManager.StatusManager;
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
                Debug.Log("Shoot" + timer);

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
        Debug.Log("Instantiate hitbox");

        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = Vector3.one * projectileSize;
        Vector3 pos = _vallis.position;
        pos.z += prefab.PreFabPosition.z;
        hitbox.transform.position = pos;
        hitbox.transform.rotation = _vallis.rotation;

        DamageContext context = new(
            damage, 
            damage,
            prefab.PrefabDuration, 
            hitShield,
            damageType,
            unitsToHit,
            _statusManager,
            new() {
                { ExtraDamageContextAtributes.Speed, projectileSpeed },
                { ExtraDamageContextAtributes.Distance, projectileDistance },
            }

            );

        ProjectileDamageHitBox projectileDamageHitBox = hitbox.GetComponent<ProjectileDamageHitBox>();
        projectileDamageHitBox.Initialize( context );
    }

    void InstantiateVFX(SkillAnimationEvent prefab) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName,
                        prefab.PreFab, TypeOfSkillPrefab.VFX);
        preFab.transform.SetPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }

    IEnumerator CooldownBetweenAttacks()
    {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(1);
    }
}
