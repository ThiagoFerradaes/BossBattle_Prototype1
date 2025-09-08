using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / Stalactite")]
public class KrakenStalactiteAttack : EnemyBehaviourSO {
    [Foldout("Attack Atributes"), SerializeField] float attackDuration;
    [Foldout("Attack Atributes"), SerializeField] float cooldownBetweenEachStalactite;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteFallSpeed;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteFallDuration;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteMinDamage;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteMaxDamage;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteMinRange;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteMaxRange;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteHeight;
    [Foldout("Attack Atributes"), SerializeField] DamageType damageType;
    [Foldout("Attack Atributes"), SerializeField] List<Tags> tags;

    [Foldout("Cooldown"), SerializeField] float smallCooldown;
    [Foldout("Cooldown"), SerializeField] float cooldownBetweenStalactiteAndNextAttack;

    [Foldout("Condition"), Range(0, 100)][SerializeField] float healthLimit;

    [Foldout("HitBox"), SerializeField] string stalactitePrefabName;
    [Foldout("HitBox"), SerializeField] GameObject stalactitePrefab;

    [Foldout("Warning"), SerializeField] string warningPrefabName;
    [Foldout("Warning"), SerializeField] float warningDuration;
    [Foldout("Warning"), SerializeField] float warningHeight;
    [Foldout("Warning"), SerializeField] GameObject stalactiteWarning;

    [Foldout("Animation"), SerializeField] string stalactiteParameterName;
    [Foldout("Animation"), SerializeField] float stalactiteAnimDuration;

    KrakenManager _krakenManager;
    bool _canAttack;
    public override void StartState(EnemyBehaviourManager parent) {

        _krakenManager = parent as KrakenManager;

        if (ReturnIfHealthIsUpToTheCondition()) {
            _krakenManager.CooldownManager.SetSkillCooldown(this, smallCooldown);
            _krakenManager.ChangeBehaviourAtRandom();
        }
        else {       

            _krakenManager.StartCoroutine(StalactiteAnimation());
        }

    }

    bool ReturnIfHealthIsUpToTheCondition() {
        return _krakenManager.ReturnCurrentHealth() > _krakenManager.ReturnMaxHealth() * (healthLimit / 100);
    }

    IEnumerator StalactiteAnimation() {
        
        foreach(var tentacle in _krakenManager.ListOfTentacles) {
            tentacle.Anim.SetBool(stalactiteParameterName, true);
        }

        yield return new WaitForSeconds(stalactiteAnimDuration);

        foreach (var tentacle in _krakenManager.ListOfTentacles) {
            tentacle.Anim.SetBool(stalactiteParameterName, false);
        }

        _krakenManager.CooldownManager.SetSkillCooldown(this);
        _canAttack = true;

        _krakenManager.StartCoroutine(StalactiteAttack());

        yield return new WaitForSeconds(cooldownBetweenStalactiteAndNextAttack);

        _krakenManager.ChangeBehaviourAtRandom();
    }
    IEnumerator StalactiteAttack() {

        _krakenManager.StartCoroutine(Duration());

        while (_canAttack) {
            Vector2 pos = ReturnAPosition();
            Vector3 stalactitePosition = new(pos.x, stalactiteHeight, pos.y);

            GameObject stalactite = PoolingManager.Instance.ReturnPrefabFromPool(stalactitePrefabName,
                stalactitePrefab, TypeOfSkillPrefab.Hitbox);
            stalactite.transform.position = stalactitePosition;

            GameObject warningVFX = PoolingManager.Instance.ReturnPrefabFromPool(warningPrefabName,
               stalactiteWarning, TypeOfSkillPrefab.VFX);

            float floorHeight = FindGroundHeight(stalactitePosition);
            Vector3 warningPos = new(pos.x, floorHeight + warningHeight, pos.y);

            warningVFX.transform.position = warningPos;
            warningVFX.GetComponent<VFXPreFab>().Initialize(warningDuration);
            _krakenManager.StartCoroutine(StalactiteFall(stalactite));

            yield return new WaitForSeconds(cooldownBetweenEachStalactite);
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
        yield return new WaitForSeconds(attackDuration);
        _canAttack = false;
    }

    IEnumerator StalactiteFall(GameObject stalactite) {
        stalactite.SetActive(true);

        DamageContext context = new(
            stalactiteMinDamage,
            stalactiteMaxDamage,
            stalactiteFallDuration,
            true,
            damageType,
            tags,
            _krakenManager.KrakenStatus
        );

        stalactite.GetComponent<InstantDamageHitBox>().Initialize(context);

        float timer = 0f;

        while (timer < stalactiteFallDuration) {
            timer += Time.deltaTime;
            stalactite.transform.position += stalactiteFallSpeed * Time.deltaTime * Vector3.down;
            yield return null;
        }

        PoolingManager.Instance.ReturnObjectToPool(stalactite, TypeOfSkillPrefab.Hitbox);
    }

    Vector2 ReturnAPosition() {
        Vector2 direction = Random.insideUnitCircle.normalized;

        float distance = Mathf.Sqrt(Random.Range(stalactiteMinRange * stalactiteMinRange,
                                             stalactiteMaxRange * stalactiteMaxRange));

        return direction * distance;
    }

}
