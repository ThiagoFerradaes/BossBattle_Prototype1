using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ BigClaw")]
public class CrabBigClawAttack : EnemyBehaviourSO
{

    CrabManager _crabManager;
    Animator _anim;
    StatusManager _statusManager;

    [Header("Animation")]
    [SerializeField] string preparingAnimationTrigger;
    [SerializeField] string attackAnimationName;
    [SerializeField] string preparingAnimationSpeedParameter;
    [SerializeField] string attackAnimationSpeedParameter;
    [SerializeField] float preparingAnimationSpeed;
    [SerializeField] float attackAnimationSpeed;
    [SerializeField] int animationLayer;

    [Header("Walk Atributes")]
    [SerializeField] float distanceToPlayer = 2;
    [SerializeField] float cooldownBetweenThisAttackAndNext = 2;
    [SerializeField] float rotationSpeed = 150;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Damage Atributes")]
    [SerializeField] float damage;
    [SerializeField] bool hitShield;
    [SerializeField] DamageType damageType;
    [SerializeField] List<Tags> unitsToHit;


    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        if (CrabArenaManager.Instance.ReturnCurrentTide() != CrabArenaState.LowTide)
        {
            _crabManager.CooldownManager.SetSkillCooldown(this);
            _crabManager.ChangeBehaviourAtRandom();
        }
        else
        {
            _crabManager.StartCoroutine(WalkToPlayer());
        }

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

        yield return _crabManager.RotateToPlayer(_crabManager.BigClaw, rotationSpeed);

        _anim.SetTrigger(preparingAnimationTrigger);
        _anim.SetFloat(preparingAnimationSpeedParameter, preparingAnimationSpeed);
        _anim.SetFloat(attackAnimationSpeedParameter, attackAnimationSpeed);

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
        hitbox.transform.SetParent(_crabManager.transform, false);
        hitbox.transform.SetLocalPositionAndRotation(prefab.PreFabPosition, Quaternion.identity);

        DamageContext context = new(
            damage, damage, prefab.PrefabDuration,
            hitShield, damageType,
            unitsToHit, _statusManager
            );

        InstantDamageHitBox damageHitBox = hitbox.GetComponent<InstantDamageHitBox>();
        damageHitBox.Initialize( context );
    }

    void InstantiateVFX(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.VFX);
        hitbox.transform.position = prefab.PreFabPosition;

        hitbox.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }
}
