using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] float playerPositionYOffSet;
    [SerializeField] Vector3 preparingAttackSize;
    [SerializeField] LayerMask layersToHit;
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

    #region Attack
    IEnumerator WaterSunAttackRoutine() {
        _anim.SetTrigger(preparingAnimationParameter);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do {
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(preparingAnimationName));

        yield return _crabManager.StartCoroutine(PreparingAttack());

        _anim.SetBool(attackAnimationParameter, true);

        do {
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(attackAnimationName));

        yield return _crabManager.StartCoroutine(AttackRoutine(stateInfo));

        EndAttack();
    }

    IEnumerator PreparingAttack() {
        List<GameObject> listOfPreparingGameObjects = new();
        if (prefabs != null) {

            var prefab = prefabs[0];

            foreach (var skillEvent in prefab) {
                if (skillEvent.PrefabType == TypeOfSkillPrefab.Hitbox) { GameObject hitbox = InstantiateHitBox(skillEvent, preparingAttackSize); listOfPreparingGameObjects.Add(hitbox); }
                else InstantiateVFX(skillEvent);
            }
        }

        foreach (var obj in listOfPreparingGameObjects) {
            obj.SetActive(true);
        }

        float timer = 0f;

        while (timer < attackPreparationDuration) {
            timer += Time.deltaTime;

            Vector3 playerPos = _crabManager.Player.transform.position;
            playerPos.y += playerPositionYOffSet;
            Vector3 dir = (playerPos - _apicem.position).normalized;

            _apicem.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            if (Physics.Raycast(_apicem.transform.position, dir, out RaycastHit hit, attackSize.z, layersToHit)) {
                foreach (var obj in listOfPreparingGameObjects) {
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, hit.distance);
                }
            }
            else {
                foreach (var obj in listOfPreparingGameObjects) {
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, attackSize.z);
                }
            }

            yield return null;
        }

        foreach (var obj in listOfPreparingGameObjects) {
            obj.transform.SetParent(null);
            obj.SetActive(false);
        }
    }

    IEnumerator AttackRoutine(AnimatorStateInfo stateInfo) {
        List<GameObject> listOfGameObjects = new();

        int attackHash = stateInfo.fullPathHash;

        if (prefabs != null) {
            var prefab = prefabs[1];
            foreach (var skillEvent in prefab) {

                do {
                    stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
                    yield return null;
                } while (attackHash == stateInfo.fullPathHash && stateInfo.normalizedTime < skillEvent.TimeToSpawnPreFab);

                if (skillEvent.PrefabType == TypeOfSkillPrefab.Hitbox) { GameObject hitbox = InstantiateHitBox(skillEvent, attackSize); listOfGameObjects.Add(hitbox); }
                else InstantiateVFX(skillEvent);
            }
        }

        float timer = 0f;

        while (timer < attackDuration) {
            timer += Time.deltaTime;

            if (Physics.Raycast(_apicem.transform.position, _apicem.transform.forward, out RaycastHit hit, attackSize.z, layersToHit)) {
                foreach (var obj in listOfGameObjects) {
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, hit.distance);
                }
            }
            else {
                foreach (var obj in listOfGameObjects) {
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, attackSize.z);
                }
            }
            yield return null;
        }
    }

    void EndAttack() {

        _anim.SetBool(attackAnimationParameter, false);

        _crabManager.CooldownManager.SetSkillCooldown(this);

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());
    }
    #endregion

    #region Instantiate
    GameObject InstantiateHitBox(SkillAnimationEvent prefab, Vector3 size) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = size;
        hitbox.transform.SetParent(_apicem.transform, false);
        Vector3 pos = prefab.PreFabPosition;

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
        if (hitbox.TryGetComponent<ContinuosDamageHitBox>(out ContinuosDamageHitBox damageHitBox)) { damageHitBox.Initialize(newContext); }

        return hitbox;
    }

    void InstantiateVFX(SkillAnimationEvent prefab) { }

    #endregion
}
