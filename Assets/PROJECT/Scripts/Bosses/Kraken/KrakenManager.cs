using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.LookDev;
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

    [SerializeField] Material deadTentacleMaterial;
    [SerializeField] KrakenTentacleAttack tentacleAttack;
    [SerializeField] BossRewardSO bossReward;
    public List<GameObject> TentaclesListGO = new();

    Dictionary<int, Coroutine> _listOfTentaclesInAnimation = new();
    Dictionary<int, bool> _listOfTentaclesDead = new();

    [HideInInspector] public List<KrakenTentacle> ListOfTentacles = new();
    [HideInInspector] public Transform Player;

    #endregion

    #region Initialize
    private void Awake() {

        for (int i = 0; i < TentaclesListGO.Count; i++) {
            KrakenTentacle newTentacle = new(TentaclesListGO[i]);
            ListOfTentacles.Add(newTentacle);

            _listOfTentaclesInAnimation[i] = null;
            _listOfTentaclesDead[i] = false;
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

    public bool IsTentacleInAnimation(int tentacleIndex) {
        if (_listOfTentaclesInAnimation[tentacleIndex] == null) return false;
        else return true;
    }

    public Coroutine ReturnTentacleCoroutine(int tentacleIndex) => _listOfTentaclesInAnimation[tentacleIndex];
    public void StartTentacleAttack(int tentacleIndex, float preparingSpeed, float hitSpeed, float downTime) {
        _listOfTentaclesInAnimation[tentacleIndex] = StartCoroutine(TentacleAttack(tentacleIndex, preparingSpeed, hitSpeed, downTime));
    }
    public IEnumerator TentacleAttack(int tentacleIndex, float preparingSpeed, float hitSpeed, float downTime) {


        Animator anim = ListOfTentacles[tentacleIndex].Anim;
        anim.SetFloat(tentacleAttack.PreparingAttackSpeed, preparingSpeed);
        anim.SetTrigger(tentacleAttack.AttackAnimationParameter);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do { // PREPARING ANIMATION
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(tentacleAttack.AttackAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        for (int i = 0; i < tentacleAttack.PrefabsPreparingAnimation.Count; i++) {
            SkillAnimationEvent prefabInfo = tentacleAttack.PrefabsPreparingAnimation[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar VFX
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) {
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                float yRotation = 180 + (tentacleIndex * 45);
                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(0, yRotation + 22.5f, 0));
                ParticleSystem ps = attackHitBox.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = preparingSpeed;
                attackHitBox.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }

        }

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        do { // HIT ANIMATION
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(tentacleAttack.AttackHitAnimationName));

        anim.SetFloat(tentacleAttack.HitAttackSpeed, hitSpeed);

        attackStateHash = stateInfo.fullPathHash;
        tentacleAttack.PrefabsHitAnimation.Sort((a, b) => a.TimeToSpawnPreFab.CompareTo(b.TimeToSpawnPreFab));


        // Instanciando todas as hitboxes
        for (int i = 0; i < tentacleAttack.PrefabsHitAnimation.Count; i++) {
            SkillAnimationEvent prefabInfo = tentacleAttack.PrefabsHitAnimation[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar hit box
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            float yRotation = 180 + (tentacleIndex * 45);

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.Hitbox) {
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(90, yRotation, 0));
                InstantDamageContext newContext = new(
                tentacleAttack.DeadTentacleDamage,
                tentacleAttack.DeadTentacleDamage,
                0.1f,
                0,
                false,
                DamageType.Abyssal,
                Tags.Player,
                ListOfTentacles[tentacleIndex].Status
                );

                attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);
            }
            else {
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFabName,
                    prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(-90, yRotation + 202.5f, 0));
                ParticleSystem ps = attackHitBox.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = preparingSpeed;
                attackHitBox.GetComponent<VFXPreFab>().Initialize(prefabInfo.PrefabDuration);
            }

        }

        ListOfTentacles[tentacleIndex].HitBox.SetActive(true);


        // Tempo vulnerável
        yield return new WaitForSeconds(downTime);

        // Voltando pro Idle
        anim.SetTrigger(tentacleAttack.ReturnToIdleAnimationParameter);
        anim.SetFloat(tentacleAttack.PreparingAttackSpeed, 1);
        anim.SetFloat(tentacleAttack.HitAttackSpeed, 1);

        ListOfTentacles[tentacleIndex].HitBox.SetActive(false);
    }
    #endregion

    #region Others
    void CheckTentaclesHealth(int tentacleId) {
        _listOfTentaclesDead[tentacleId] = true;
        ListOfTentacles[tentacleId].SkinnedMeshRenderer.material = deadTentacleMaterial;

        bool allTentaclesDead = true;
        foreach (var tentacle in _listOfTentaclesDead) {
            bool dead = tentacle.Value;
            if (!dead) {
                allTentaclesDead = false;
            }
        }

        if (allTentaclesDead) {
            if(bossReward != null) bossReward.WinRewards();
            ScreensInGameUI.Instance.TurnScreenOn(TypeOfScreen.Victory);
        }
    }

    #endregion

}
