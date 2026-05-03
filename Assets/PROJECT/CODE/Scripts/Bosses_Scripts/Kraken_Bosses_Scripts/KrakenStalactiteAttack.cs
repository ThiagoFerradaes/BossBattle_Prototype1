using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ Behaviour/ Kraken / Stalactite")]
public class KrakenStalactiteAttack : EnemyBehaviourSO {
    [Foldout("Attack Atributes"), SerializeField] float attackDuration;
    [Foldout("Attack Atributes"), SerializeField] float cooldownBetweenEachStalactite;
    [Foldout("Attack Atributes"), SerializeField] float cooldownBetweenWarningAndStalactite;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteFallSpeed;
    [Foldout("Attack Atributes"), SerializeField] float stalactiteHeight;
    [Foldout("Attack Atributes"), SerializeField] float floorHeight;
    [Foldout("Attack Atributes"), SerializeField] DamageAtributes damageAtributes;
    [Foldout("Attack Atributes"), SerializeField] AK.Wwise.Event stalactiteExplosionSound;

    [Foldout("Cooldown"), SerializeField] float smallCooldown;
    [Foldout("Cooldown"), SerializeField] float cooldownBetweenStalactiteAndNextAttack;

    [Foldout("Condition"), Range(0, 100)][SerializeField] float healthLimit;

    [Foldout("HitBox"), SerializeField] GameObject stalactitePrefab;

    [Foldout("Warning"), SerializeField] VFXAtributes warningVFXAtributes;
    [Foldout("Warning"), SerializeField] float warningHeight;
    [Foldout("Warning"), SerializeField] GameObject stalactiteWarning;

    [Foldout("Animation"), SerializeField] string stalactiteParameterName;
    [Foldout("Animation"), SerializeField] float stalactiteAnimDuration;

    KrakenManager _krakenManager;
    bool _canAttack;

    WaitForSeconds myWaitForSeconds = new(0.3f);
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
            Vector3 pos = ArenaManager.Instance.GetRandomPosition(1);
            Vector3 stalactitePosition = new(pos.x, stalactiteHeight, pos.z);

            GameObject stalactite = PoolingManager.Instance.ReturnPrefabFromPool(stalactitePrefab, TypeOfSkillPrefab.Hitbox);
            stalactite.transform.position = stalactitePosition;
            stalactite.GetComponent<Collider>().enabled = true;
            stalactite.GetComponentInChildren<MeshRenderer>().enabled = true;

            GameObject warningVFX = PoolingManager.Instance.ReturnPrefabFromPool(stalactiteWarning, TypeOfSkillPrefab.VFX);

            float floorHeight = ArenaManager.Instance.FindGroundHeight(stalactitePosition); 
            Vector3 warningPos = new(pos.x, floorHeight + warningHeight, pos.z);

            warningVFX.transform.position = warningPos;
            warningVFX.GetComponent<VFXPreFabStatic>().Initialize(warningVFXAtributes);

            _krakenManager.StartCoroutine(StalactiteFall(stalactite));

            yield return new WaitForSeconds(cooldownBetweenEachStalactite);
        }
    }

    IEnumerator Duration() {
        yield return new WaitForSeconds(attackDuration);
        _canAttack = false;
    }

    IEnumerator StalactiteFall(GameObject stalactite) {


        stalactite.SetActive(true);
        yield return new WaitForSeconds(cooldownBetweenWarningAndStalactite);
        
        DamageContext context = new(
            damageAtributes,
            _krakenManager.KrakenStatus
        );

        stalactite.GetComponent<InstantDamageHitBox>().Initialize(context, false);

        while (stalactite.transform.position.y >= floorHeight) {
            stalactite.transform.position += stalactiteFallSpeed * Time.deltaTime * Vector3.down;
            yield return null;
        }
        stalactite.GetComponent<Collider>().enabled = false;
        stalactite.GetComponentInChildren<MeshRenderer>().enabled = false;
        stalactiteExplosionSound.Post(stalactite);
        yield return myWaitForSeconds;
        PoolingManager.Instance.ReturnObjectToPool(stalactite, TypeOfSkillPrefab.Hitbox);
    }

}
