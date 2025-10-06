using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaterSun")]
public class CrabWaterSunAttack : EnemyBehaviourSO {
    CrabManager _crabManager;
    Animator _anim;
    Transform _apicem;
    StatusManager _statusManager;

    [Header("Animation")]
    [SerializeField] string preparingAnimationParameter;
    [SerializeField] string preparingAnimationName;
    [SerializeField] string attackAnimationParameter;
    [SerializeField] string attackAnimationName;
    [SerializeField] int animationLayer;

    [Header("Preparing attack atributes")]
    [SerializeField] float attackPreparationDuration;
    [SerializeField] float cooldownBetweenAttacks;
    [SerializedDictionary("Combo", "List"), SerializeField] SerializedDictionary<int, List<SkillAnimationEvent>> prefabs;

    [Header("Attack atributes")]
    [SerializeField] Vector3 attackSize;
    [SerializeField] float attackDuration;
    [SerializeField] float attackDamageCooldown;
    [SerializeField] float damage;
    [SerializeField] bool hitShield;
    [SerializeField] List<Tags> unitsToHit;
    [SerializeField] DamageType damageType;


    #region Initialize
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WaterSunAttackRoutine());
    }

    public override bool MeetsCondition() {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent) {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _apicem = _crabManager.Apicem;
        _statusManager = _crabManager.StatusManager;
    }
    #endregion

    IEnumerator WaterSunAttackRoutine() {
        _anim.SetTrigger(preparingAnimationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do {
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(preparingAnimationName));

        float timer = 0f;

        while (timer < attackPreparationDuration) {
            timer += Time.deltaTime;

            Vector3 dir = (_crabManager.Player.transform.position - _apicem.position).normalized;
            dir.y = 0;

            _apicem.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            yield return null;
        }

        _anim.SetBool(attackAnimationParameter, true);

        do {
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(attackAnimationName));

        int attackHash = stateInfo.fullPathHash;

        if (prefabs != null) {
            var prefab = prefabs[1];
            foreach(var skillEvent in prefab) {

                do {
                    stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
                    yield return null;
                } while (attackHash == stateInfo.fullPathHash && stateInfo.normalizedTime < skillEvent.TimeToSpawnPreFab);

                if (skillEvent.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(skillEvent);
                else InstantiateVFX(skillEvent);
            }
        }

        yield return new WaitForSeconds(attackDuration);

        _anim.SetBool(attackAnimationParameter, false);

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(Channel);
    }

    void InstantiateHitBox(SkillAnimationEvent prefab) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = attackSize;
        hitbox.transform.SetParent(_apicem.transform, false);
        Vector3 pos = new(prefab.PreFabPosition.x, prefab.PreFabPosition.y, attackSize.z/2);

        hitbox.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);

        DamageContext newContext = new(
           damage,
           damage,
           attackDuration,
           hitShield,
           damageType,
           unitsToHit,
           _statusManager,
           new() {
                {ExtraDamageContextAtributes.DamageCooldown, attackDamageCooldown}
           }
           );
        ContinuosDamageHitBox hitBox = hitbox.GetComponent<ContinuosDamageHitBox>();

        hitBox.Initialize(newContext);
    }

    void InstantiateVFX(SkillAnimationEvent prefab) { }
}
