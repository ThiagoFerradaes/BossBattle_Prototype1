using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KrakenTentacle {
    public Animator Anim;
    public HealthManager Health;
    public GameObject HitBox;
    public SkinnedMeshRenderer SkinnedMeshRenderer;
    public StatusManager Status;

    public KrakenTentacle(GameObject tentacle) {
        Anim = tentacle.GetComponentInChildren<Animator>();
        SkinnedMeshRenderer = tentacle.GetComponentInChildren<SkinnedMeshRenderer>();

        foreach (Transform child in tentacle.transform) {
            if (child.gameObject.CompareTag("Enemy")) {
                HitBox = child.gameObject;
            }
        }

        Health = HitBox.GetComponent<HealthManager>();
        Status = HitBox.GetComponent<StatusManager>();
    }
}
public class KrakenManager : EnemyBehaviourManager {

    #region Parameters

    [Foldout("Generic Atributes"), SerializeField] float cooldownBetweenAttacks;
    [Foldout("Generic Atributes"), SerializeField] Material deadTentacleMaterial;

    [Foldout("Lists"), SerializeField] List<EnemyBehaviourSO> _listOfSkills = new();
    [Foldout("Lists")] public List<GameObject> TentaclesListGO = new();
    public List<KrakenTentacle> ListOfTentacles = new();
    int tentaclesDead = 0;

    [SerializeField] KrakenTentacleAttack tentacleAttack;

    [HideInInspector] public Transform Player;

    #endregion

    #region Initialize
    private void Awake() {

        for (int i = 0; i < TentaclesListGO.Count; i++) {
            KrakenTentacle newTentacle = new(TentaclesListGO[i]);
            ListOfTentacles.Add(newTentacle);
        }

    }

    public override void Start() {

        Player = PlayerManager.Instance.Player.transform;

        for (int i = 0; i < ListOfTentacles.Count; i++) {
            int tentacleIndex = i;
            ListOfTentacles[i].Health.OnDeath += () => CheckTentaclesHealth(tentacleIndex);
        }

        base.Start();
    }

    private void OnDestroy() {
        for (int i = 0; i < ListOfTentacles.Count; i++) {
            int tentacleIndex = i;
            ListOfTentacles[i].Health.OnDeath -= () => CheckTentaclesHealth(tentacleIndex);
        }
    }
    #endregion

    #region Generic Attacks

    public int FindClosestTentacleToPlayer() {
        int tentacleIndex = -1;
        float distance = Mathf.Infinity;

        for (int i = 0; i < TentaclesListGO.Count; i++) {
            if (TentaclesListGO[i] == null) continue;

            float newDistance = Vector3.Distance(TentaclesListGO[i].transform.position, Player.position);
            if (newDistance < distance) {
                distance = newDistance;
                tentacleIndex = i;
            }
        }

        return tentacleIndex;
    }

    public IEnumerator TentacleAttack(int tentacleIndex) {

        Animator anim = ListOfTentacles[tentacleIndex].Anim;
        anim.SetTrigger(tentacleAttack.AttackAnimationParameter);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do { // Esperando a primeira animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(tentacleAttack.AttackAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        do { // Esperando a segunda animação
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(tentacleAttack.AttackHitAnimationName));

        attackStateHash = stateInfo.fullPathHash;
        tentacleAttack.Prefabs.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));

        for (int i = 0; i < tentacleAttack.Prefabs.Count; i++) {
            SkillAnimationEvent prefabInfo = tentacleAttack.Prefabs[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(prefabInfo.PreFabName, prefabInfo.PreFab);
            float yRotation = 180 + (tentacleIndex * 45);
            attackHitBox.transform.SetPositionAndRotation(new Vector3(0, 3, 0), Quaternion.Euler(90, yRotation, 0));

            if (prefabInfo.PrefabType == TypeOfSkillAnimationPrefab.Hitbox) {

                InstantDamageContext newContext = new(
                tentacleAttack.TentacleDamage,
                0.1f,
                0,
                false,
                DamageType.Physical,
                Tags.Player,
                ListOfTentacles[tentacleIndex].Status
                );

                attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

            }
        }

        ListOfTentacles[tentacleIndex].HitBox.SetActive(true);

        yield return new WaitForSeconds(3);

        anim.SetTrigger(tentacleAttack.ReturnToIdleAnimationParameter);

        ListOfTentacles[tentacleIndex].HitBox.SetActive(false);
    }
    #endregion

    #region Specific Attacks

    IEnumerator StalactiteAttack(EnemyBehaviourSO skill) {
        float timer = 0f;

        KrakenStalactiteAttack info = skill as KrakenStalactiteAttack;

        Queue<Vector3> lastPositions = new Queue<Vector3>();

        while (timer < info.AttackDuration) {
            timer += Time.deltaTime;

            if (timer % info.CooldownBetweenEachStalactite < Time.deltaTime) {
                SpawnObject(lastPositions, info);
            }

            yield return null;
        }
    }

    void SpawnObject(Queue<Vector3> lastPositions, KrakenStalactiteAttack info) {
        Vector3 spawnPos;
        int tries = 0;
        do {
            Vector2 randomPos2D = Random.insideUnitCircle * info.StalactiteRange;
            spawnPos = new Vector3(randomPos2D.x, info.StalactiteHeight, randomPos2D.y);

            tries++;
            if (tries > 20) 
                break;

        } while (IsTooCloseToLast(spawnPos, lastPositions, info));

        lastPositions.Enqueue(spawnPos);
        if (lastPositions.Count > 5)
            lastPositions.Dequeue();

        GameObject prefab = SkillPoolingManager.Instance.ReturnHitboxFromPool(info.Prefabs[0].PreFabName, info.Prefabs[0].PreFab);
        //StartCoroutine(StalactiteFalling(prefab, spawnPos));
    }

    bool IsTooCloseToLast(Vector3 pos, Queue<Vector3> lastPositions, KrakenStalactiteAttack info) {
        foreach (var last in lastPositions) {
            if (Vector3.Distance(pos, last) < info.StalactiteDistanceFromEachOther) return true;
        }
        return false;
    }
    //IEnumerator StalactiteFalling(GameObject stalactite, Vector3 pos) {

    //}

    #endregion

    #region Others
    void CheckTentaclesHealth(int tentacleId) {
        tentaclesDead++;
        ListOfTentacles[tentacleId].SkinnedMeshRenderer.material = deadTentacleMaterial;
        if (tentaclesDead == TentaclesListGO.Count) ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Victory);
    }

    #endregion

}
