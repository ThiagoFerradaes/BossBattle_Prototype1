using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KrakenTentacle {

    // Components
    public Animator Anim;
    public HealthManager Health;
    public GameObject HitBox;
    public SkinnedMeshRenderer MeshRend;
    public StatusManager Status;

    public KrakenTentacle(GameObject tentacle) {
        Anim = tentacle.GetComponentInChildren<Animator>();
        MeshRend = tentacle.GetComponentInChildren<SkinnedMeshRenderer>();

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

    [Header("Components")]
    [SerializeField] Material deadTentacleMaterial;
    [SerializeField] KrakenTentacleAttack tentacleAttack;
    [HideInInspector] public List<KrakenTentacle> ListOfTentacles = new();
    [HideInInspector] public Transform Player;
    public StatusManager KrakenStatus;

    [Header("Lists")]
    public List<GameObject> TentaclesListGO = new();

    // Lists
    Dictionary<int, Coroutine> _listOfTentaclesInAnimation = new();
    Dictionary<int, bool> _listOfTentaclesDead = new();

    // Atributes
    float _maxHealth, _currentHealth;

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

    public override IEnumerator Start() {

        yield return new WaitForEndOfFrame();

        Player = PlayerManager.Instance.Player.transform;

        for (int i = 0; i < ListOfTentacles.Count; i++) {
            int tentacleIndex = i;
            ListOfTentacles[i].Health.OnDeath += () => CheckTentaclesHealth(tentacleIndex);
            ListOfTentacles[i].Health.OnDamageTaken += HandleChangeInHealth;

            _maxHealth += ListOfTentacles[i].Health.ReturnMaxHealth();
        }

        _currentHealth = _maxHealth;

        StartCoroutine(base.Start());
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

        int attackHash = Animator.StringToHash(tentacleAttack.AttackAnimationName);
        anim.CrossFade(attackHash, 0.1f, 0);

        while(anim.IsInTransition(0)) yield return null;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        int attackStateHash = stateInfo.fullPathHash;

        for (int i = 0; i < tentacleAttack.PrefabsPreparingAnimation.Count; i++) {
            SkillAnimationEvent prefabInfo = tentacleAttack.PrefabsPreparingAnimation[i];
            float targetNormalizedTime = prefabInfo.TimeToSpawnPreFab;

            do { // Esperando o tempo para instanciar VFX
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < targetNormalizedTime);

            if (prefabInfo.PrefabType == TypeOfSkillPrefab.VFX) {
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);

                float yRotation = 180 + (tentacleIndex * 45);
                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(0, yRotation + 22.5f, 0));

                ParticleSystem ps = attackHitBox.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = preparingSpeed;

                attackHitBox.GetComponent<VFXPreFabStatic>().Initialize(prefabInfo.VFXAtribute);
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
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.Hitbox);

                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(90, yRotation, 0));
                attackHitBox.transform.localScale = Vector3.one * tentacleAttack.TentacleAttackSize;
                DamageAtributes atributes = ListOfTentacles[tentacleIndex].Health.ReturnIfIsDead() ?
                    tentacleAttack.DeadTentacleDamageAtributes : tentacleAttack.AliveTentacleDamageAtributes;

                DamageContext newContext = new(
                atributes,
                ListOfTentacles[tentacleIndex].Status
                );

                attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);
            }
            else {
                GameObject attackHitBox = PoolingManager.Instance.ReturnPrefabFromPool(prefabInfo.PreFab, TypeOfSkillPrefab.VFX);
                attackHitBox.transform.SetPositionAndRotation(prefabInfo.PreFabPosition, Quaternion.Euler(-90, yRotation + 202.5f, 0));
                ParticleSystem ps = attackHitBox.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = preparingSpeed;
                attackHitBox.GetComponent<VFXPreFabStatic>().Initialize(prefabInfo.VFXAtribute);
            }

        }

        ListOfTentacles[tentacleIndex].HitBox.SetActive(true);

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        // Tempo vulnerável
        float vulnerableTime = ListOfTentacles[tentacleIndex].Health.ReturnIfIsDead() ? 0.2f : downTime;

        yield return new WaitForSeconds(vulnerableTime);

        // Voltando pro Idle
        anim.SetTrigger(tentacleAttack.ReturnToIdleAnimationParameter);
        anim.SetFloat(tentacleAttack.PreparingAttackSpeed, 1);
        anim.SetFloat(tentacleAttack.HitAttackSpeed, 1);

        do { // Return to idle ANIMATION
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(tentacleAttack.ReturnToIdleAnimationName));

        attackStateHash = stateInfo.fullPathHash;

        do { // Esperando o tempo para desligar a hitbox
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash &&
        stateInfo.normalizedTime < tentacleAttack.TimeInReturnToIdleToTurnOffHitBox);

        ListOfTentacles[tentacleIndex].HitBox.SetActive(false);

        do { // Esperando o tempo de retorno para idle
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < 1);
    }
    #endregion

    #region Others
    void CheckTentaclesHealth(int tentacleId) {
        _listOfTentaclesDead[tentacleId] = true;
        ListOfTentacles[tentacleId].MeshRend.material = deadTentacleMaterial;

        bool allTentaclesDead = true;
        foreach (var tentacle in _listOfTentaclesDead) {
            bool dead = tentacle.Value;
            if (!dead) {
                allTentaclesDead = false;
            }
        }

        if (allTentaclesDead) {
            VictoryScreenManager.Instance.InitializeVictoryScreen();
            ProgressWhiteBoard.Instance.SetProgressBool(ProgressBools.IsKrakenDefeated, true);
        }
    }

    private void HandleChangeInHealth(float damage) {
        _currentHealth -= damage;
    }

    #endregion

    #region Getters

    public float ReturnCurrentHealth() => _currentHealth;
    public float ReturnMaxHealth() => _maxHealth;

    #endregion

}
