using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ SmalClaw")]
public class CrabSmallClawAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    StatusManager _statusManager;

    [Header("Animation")]
    [SerializeField] string preparingAnimationTrigger;
    [SerializeField] string attackAnimationName;
    [SerializeField] string walkAnimationParameter;
    [SerializeField] float preparingAnimationSpeed;
    [SerializeField] float attackAnimationSpeed;
    [SerializeField] int animationLayer;

    [Header("Walk Atributes")]
    [SerializeField] float distanceToPlayer = 2;
    [SerializeField] float cooldownBetweenThisAttackAndNext = 2;
    [SerializeField] float rotationSpeed = 6;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Attack Atributes")]
    [SerializeField] float amoutOfAttacks;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float angleOfAttack;
    [SerializeField] Vector3 sizeOfHitbox;

    [Header("Damage Atributes")]
    [SerializeField] float damage;
    [SerializeField] bool hitShield;
    [SerializeField] DamageType damageType;
    [SerializeField] List<Tags> unitsToHit;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WalkToPlayer());

    }

    public override bool MeetsCondition()
    {
        return CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide;
    }
    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _statusManager = _crabManager.StatusManager;
    }

    IEnumerator WalkToPlayer()
    {

        _crabManager.WalkToTarget(distanceToPlayer, _crabManager.Player.transform.position);

        yield return _crabManager.ReturnWalkCoroutine();

        for (int j = 0; j < amoutOfAttacks; j++)
        {
            yield return RotateToPlayer();

            _anim.SetTrigger(preparingAnimationTrigger);

            AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

            do
            {
                yield return null;
                stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            } while (!stateInfo.IsName(attackAnimationName));

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

            if (j < amoutOfAttacks - 1) yield return new WaitForSeconds(cooldownBetweenAttacks);
        }

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks()
    {
        yield return new WaitForSeconds(cooldownBetweenThisAttackAndNext);
        _crabManager.ChangeBehaviourAtRandom();

    }

    void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = sizeOfHitbox;
        hitbox.transform.SetParent(_crabManager.transform, false);
        Vector3 hitboxPosition = new(prefab.PreFabPosition.x, prefab.PreFabPosition.y, sizeOfHitbox.z/2);
        hitbox.transform.SetLocalPositionAndRotation(hitboxPosition, Quaternion.identity);

        DamageContext context = new(
            damage, damage, prefab.PrefabDuration,
            hitShield, damageType,
            unitsToHit, _statusManager
            );

        InstantDamageHitBox damageHitBox = hitbox.GetComponent<InstantDamageHitBox>();
        damageHitBox.Initialize(context);
    }

    void InstantiateVFX(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = prefab.PreFabPosition;

        hitbox.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }

    YieldInstruction RotateToPlayer()
    {
        _anim.SetBool(walkAnimationParameter, true);

        Vector3 playerPos = _crabManager.Player.transform.position;

        Vector3 playerDir = playerPos - _crabManager.transform.position;
        playerDir.Normalize();
        playerDir.y = 0;

        Quaternion startRot = Quaternion.LookRotation(playerDir, Vector3.up);

        Quaternion offSet = startRot * Quaternion.Euler(0, angleOfAttack, 0);

        // Garante rotação só no eixo Y
        Vector3 startEuler = offSet.eulerAngles;
        offSet = Quaternion.Euler(0, startEuler.y, 0);

        float angle = Quaternion.Angle(_crabManager.transform.rotation, offSet);
        float duration = angle / rotationSpeed;

        return _crabManager.transform.DORotateQuaternion(offSet, duration).OnComplete(() => _anim.SetBool(walkAnimationParameter, false)).WaitForCompletion();

    }
}
