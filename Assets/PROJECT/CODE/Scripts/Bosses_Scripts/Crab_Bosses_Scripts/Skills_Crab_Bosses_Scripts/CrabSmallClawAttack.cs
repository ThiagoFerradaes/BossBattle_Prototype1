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
    [SerializeField] string preparingAnimationSpeedParameter;
    [SerializeField] string attackAnimationSpeedParameter;
    [SerializeField] float preparingAnimationSpeed;
    [SerializeField] float attackAnimationSpeed;
    [SerializeField] int animationLayer;

    [Header("Walk Atributes")]
    [SerializeField] float distanceToPlayer = 2;
    [SerializeField] float rotationSpeed = 6;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Attack Atributes")]
    [SerializeField] float amoutOfAttacks;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] Vector3 sizeOfHitbox;

    [Header("Damage Atributes")]
    [SerializeField] DamageAtributes damageAtributes;

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

        yield return _crabManager.StartCoroutine(Attack());

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }

    IEnumerator Attack()
    {
        for (int j = 0; j < amoutOfAttacks; j++)
        {
            yield return _crabManager.RotateToPlayer(_crabManager.SmallClaw, rotationSpeed);

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

            if (j < amoutOfAttacks - 1) yield return new WaitForSeconds(cooldownBetweenAttacks);
        }
    }

    void InstantiateHitBox(SkillAnimationEvent prefab)
    {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = sizeOfHitbox;
        hitbox.transform.SetParent(_crabManager.transform, false);
        Vector3 hitboxPosition = new(prefab.PreFabPosition.x, prefab.PreFabPosition.y, sizeOfHitbox.z/2);
        hitbox.transform.SetLocalPositionAndRotation(hitboxPosition, Quaternion.identity);

        DamageContext context = new(
            damageAtributes, 
             _statusManager
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

}
