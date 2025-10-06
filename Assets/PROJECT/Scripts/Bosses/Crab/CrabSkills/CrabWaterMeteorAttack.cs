using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaterMeteor")]
public class CrabWaterMeteorAttack : EnemyBehaviourSO {
    CrabManager _crabManager;
    Animator _anim;
    Transform _apicem;
    StatusManager _statusManager;

    [Header("Attack atributes")]
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float amountOfAttacks;
    [SerializeField] float cooldownBetweenMeteors;
    [SerializeField] float maxDistanceToPlayer;
    [SerializeField] float meteorFallSpeed;
    [SerializeField] float meteorSize;
    [SerializeField] List<SkillAnimationEvent> prefabs;

    [Header("Damage Atributes")]
    [SerializeField] float projectileDistance;
    [SerializeField] float damage;
    [SerializeField] bool hitShield;
    [SerializeField] List<Tags> unitsToHit;
    [SerializeField] DamageType damageType;

    [Header("Animation")]
    [SerializeField] string animationParameter;
    [SerializeField] string animationName;
    [SerializeField] int animationLayer;

    #region Initialize
    public override void StartState(EnemyBehaviourManager parent) {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(WaterMeteorAttackRoutine());
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

    IEnumerator WaterMeteorAttackRoutine() {

        _anim.SetBool(animationParameter, true);

        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);

        do {
            stateInfo = _anim.GetCurrentAnimatorStateInfo(animationLayer);
            yield return null;
        } while (!stateInfo.IsName(animationName));

        for (int i = 0; i < amountOfAttacks; i++) {
            Vector3 pos = FindAPosition();

            if (prefabs != null) {
                var prefabList = prefabs;
                prefabList.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

                for (int j = 0; j < prefabList.Count; j++) {
                    var prefab = prefabList[j];

                    if (prefab.PrefabType == TypeOfSkillPrefab.Hitbox) InstantiateHitBox(prefab, pos);
                    else InstantiateVFX(prefab, pos);
                }
            }

            yield return new WaitForSeconds(cooldownBetweenMeteors);
        }

        _anim.SetBool(animationParameter, false);

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }
    Vector3 FindAPosition() {
        Vector3 pos = ArenaManager.Instance.GetRandomPosition();
        pos.y = ArenaManager.Instance.FindGroundHeight(pos);
        return pos;
    }
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(Channel);
    }

    void InstantiateHitBox(SkillAnimationEvent prefab, Vector3 pos) {
        GameObject hitbox = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName, prefab.PreFab, TypeOfSkillPrefab.Hitbox);
        hitbox.transform.localScale = Vector3.one * meteorSize;
        pos.y += prefab.PreFabPosition.y;
        hitbox.transform.position = pos;
        hitbox.transform.rotation = Quaternion.Euler(90, 0, 0);

        DamageContext context = new(
            damage, damage, prefab.PrefabDuration,
            hitShield, damageType, unitsToHit, _statusManager,
            new() {
                { ExtraDamageContextAtributes.Speed, meteorFallSpeed },
                { ExtraDamageContextAtributes.Distance, projectileDistance },
            }
            );

        ProjectileDamageHitBox projectile = hitbox.GetComponent<ProjectileDamageHitBox>();
        projectile.Initialize(context);
    }
    void InstantiateVFX(SkillAnimationEvent prefab, Vector3 pos) {
        GameObject preFab = PoolingManager.Instance.ReturnPrefabFromPool(prefab.PreFabName,
                        prefab.PreFab, TypeOfSkillPrefab.VFX);
        pos.y += prefab.PreFabPosition.y;
        preFab.transform.SetPositionAndRotation(pos, Quaternion.identity);

        preFab.GetComponent<VFXPreFab>().Initialize(prefab.PrefabDuration);
    }
}
