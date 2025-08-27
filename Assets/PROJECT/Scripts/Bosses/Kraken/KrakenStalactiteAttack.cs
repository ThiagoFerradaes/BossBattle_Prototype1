using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / Stalactite")]
public class KrakenStalactiteAttack : EnemyBehaviourSO {
    public float AttackDuration;
    public float CooldownBetweenEachStalactite;
    public float StalactiteFallSpeed;
    public float StalactiteFallDuration;
    public float StalactiteDistanceFromEachOther;
    public float StalactiteMinDamage;
    public float StalactiteMaxDamage;
    public float StalactiteMinRange;
    public float StalactiteMaxRange;
    public float StalactiteHeight;
    public float SmallCooldown;
    [Range(0, 100)] public float HealthLimit;
    public string StalactitePrefabName;
    public GameObject StalactitePrefab;
    public string WarningPrefabName;
    public GameObject StalactiteWarning;
    public float WarningDuration;
    public float WarningHeight;
    public DamageType DamageType;
    public Tags Tags;

    KrakenManager _krakenManager;
    bool _canAttack;
    public override void StartState(EnemyBehaviourManager parent) {

        _krakenManager = parent as KrakenManager;

        if (_krakenManager.ReturnCurrentHealth() > _krakenManager.ReturnCurrentHealth() * (HealthLimit / 100)) {
            _krakenManager.CooldownManager.SetSkillCooldown(this, SmallCooldown);
            _krakenManager.ChangeBehaviourAtRandom();
        }
        else {       

            _krakenManager.StartCoroutine(StalactiteAnimation());
        }

    }

    IEnumerator StalactiteAnimation() {
        // Faz a animação
        yield return null;

        _krakenManager.CooldownManager.SetSkillCooldown(this);
        _canAttack = true;


        _krakenManager.StartCoroutine(StalactiteAttack());
        _krakenManager.ChangeBehaviourAtRandom();
    }
    IEnumerator StalactiteAttack() {

        _krakenManager.StartCoroutine(Duration());

        while (_canAttack) {
            Vector2 pos = ReturnAPosition();
            Vector3 stalactitePosition = new(pos.x, StalactiteHeight, pos.y);

            GameObject stalactite = PoolingManager.Instance.ReturnPrefabFromPool(StalactitePrefabName,
                StalactitePrefab, TypeOfSkillPrefab.Hitbox);
            stalactite.transform.position = stalactitePosition;

            GameObject warningVFX = PoolingManager.Instance.ReturnPrefabFromPool(WarningPrefabName,
               StalactiteWarning, TypeOfSkillPrefab.VFX);

            float floorHeight = FindGroundHeight(stalactitePosition);
            Vector3 warningPos = new(pos.x, floorHeight + WarningHeight, pos.y);

            warningVFX.transform.position = warningPos;
            warningVFX.GetComponent<VFXPreFab>().Initialize(WarningDuration);
            _krakenManager.StartCoroutine(StalactiteFall(stalactite));

            yield return new WaitForSeconds(CooldownBetweenEachStalactite);
        }
    }

    float FindGroundHeight(Vector3 originalPos) {
        Vector3 startPos = originalPos + Vector3.up * 0.5f;

        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 100, LayerMask.GetMask("Floor"))) {
            return hit.point.y + 0.5f;
        }

        return 0f;
    }
    IEnumerator Duration() {
        yield return new WaitForSeconds(AttackDuration);
        _canAttack = false;
    }

    IEnumerator StalactiteFall(GameObject stalactite) {
        stalactite.SetActive(true);

        InstantDamageContext context = new(
            StalactiteMinDamage,
            StalactiteMaxDamage,
            StalactiteFallDuration,
            0,
            true,
            DamageType,
            Tags,
            _krakenManager.KrakenStatus
        );

        stalactite.GetComponent<InstantDamageHitBox>().Initialize(context);

        float timer = 0f;

        while (timer < StalactiteFallDuration) {
            timer += Time.deltaTime;
            stalactite.transform.position += StalactiteFallSpeed * Time.deltaTime * Vector3.down;
            yield return null;
        }

        PoolingManager.Instance.ReturnObjectToPool(stalactite, TypeOfSkillPrefab.Hitbox);
    }

    Vector2 ReturnAPosition() {
        Vector2 direction = Random.insideUnitCircle.normalized;

        float distance = Mathf.Sqrt(Random.Range(StalactiteMinRange * StalactiteMinRange,
                                             StalactiteMaxRange * StalactiteMaxRange));

        return direction * distance;
    }

}
