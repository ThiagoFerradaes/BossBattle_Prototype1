using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KrakenTentacle {
    public Animator Anim;
    public HealthManager Health;
    public GameObject HitBox;

    public KrakenTentacle(GameObject tentacle) {
        Anim = tentacle.GetComponentInChildren<Animator>();

        foreach (Transform child in tentacle.transform) {
            if (child.gameObject.CompareTag("Enemy")) {
                HitBox = child.gameObject;
            }
        }

        Health = HitBox.GetComponent<HealthManager>();
    }
}
public class KrakenManager : MonoBehaviour {
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] List<EnemySkillSO> _listOfSkills = new();
    [SerializeField] List<GameObject> _tentaclesList = new();
    List<KrakenTentacle> _listOfTentacles = new();

    [SerializeField] string AttackAnimationParameter;
    [SerializeField] string ReturnToIdleAnimationParameter;
    [SerializeField] string AttackAnimationName;
    [SerializeField] string AttackHitAnimationName;
    [SerializeField] string ReturnToIdleAnimationName;

    EnemyCooldownManager _enemyCooldownManager;
    Transform _player;


    #region Initialize
    private void Awake() {
        _enemyCooldownManager = GetComponent<EnemyCooldownManager>();
        _enemyCooldownManager.Initiate(_listOfSkills);

        for (int i = 0; i < _tentaclesList.Count; i++) {
            KrakenTentacle newTentacle = new(_tentaclesList[i]);
            _listOfTentacles.Add(newTentacle);
        }

    }

    private void Start() {

        _player = PlayerSpawnManager.Instance.Player.transform;

        StartCoroutine(CooldownBetweenAttacks());
    }

    #endregion

    #region Generic Attacks
    IEnumerator CooldownBetweenAttacks() {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        ChooseAnAttack();
    }
    void ChooseAnAttack() {
        int priority = -1;
        EnemySkillSO currentSkill = null;
        foreach (var skill in _listOfSkills) {
            if (skill.Priority >= priority && !_enemyCooldownManager.SkillInCooldown(skill)) {
                currentSkill = skill;
                priority = skill.Priority;
            }
        }

        if (currentSkill != null) Attack(_listOfSkills[3], _listOfSkills.IndexOf(currentSkill));
    }

    void Attack(EnemySkillSO skill, int skillIndex) {

        _enemyCooldownManager.SetSkillCooldown(skill);

        if (skill is KrakenRandomAttack) {
            KrakenRandomAttack(skillIndex);
        }
        else if (skill is KrakenHalfAttack) {
            StartCoroutine(KrakenHalfAttackCoroutine(skillIndex));
        }
        else if (skill is KrakenSpinningAttack) {
            StartCoroutine(KrakenSpinningAttackCoroutine(skillIndex)); 
        }
        else if (skill is KrakenCrossAttack) {
            StartCoroutine(KrakenCrossAttackCoroutine(skillIndex));
        }
        else if (skill is KrakenRainAttack) {

        }
    }
    int FindClosestTentacleToPlayer() {
        int tentacleIndex = -1;
        float distance = Mathf.Infinity;

        for (int i = 0; i < _tentaclesList.Count; i++) {
            if (_tentaclesList[i] == null) continue;

            float newDistance = Vector3.Distance(_tentaclesList[i].transform.position, _player.position);
            if (newDistance < distance) {
                distance = newDistance;
                tentacleIndex = i;
            }
        }

        return tentacleIndex;
    }
    #endregion

    #region
    void KrakenRandomAttack(int skillIndex) {
        KrakenRandomAttack info = _listOfSkills[skillIndex] as KrakenRandomAttack;

        int tentacleToHit = FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _tentaclesList[tentacleToHit].transform.position;
        Vector3 playerPos = _player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        int secondTentacleIndex;

        if (cross.y < 0) {
            secondTentacleIndex = (tentacleToHit - 1 + _tentaclesList.Count) % _tentaclesList.Count;
        }
        else {
            secondTentacleIndex = (tentacleToHit + 1) % _tentaclesList.Count;
        }

        int rng = Random.Range(0, 2);

        if (rng == 1) rng = tentacleToHit;
        else rng = secondTentacleIndex;

        StartCoroutine(TentacleAttack(rng, info));

        StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator KrakenHalfAttackCoroutine(int skillIndex) {
        KrakenHalfAttack info = _listOfSkills[skillIndex] as KrakenHalfAttack;

        int tentacleToHit = FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _tentaclesList[tentacleToHit].transform.position;
        Vector3 playerPos = _player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        int secondTentacleIndex;
        bool isRight = false;

        if (cross.y < 0) {
            secondTentacleIndex = (tentacleToHit - 1 + _tentaclesList.Count) % _tentaclesList.Count;
            isRight = false;
        }
        else {
            secondTentacleIndex = (tentacleToHit + 1) % _tentaclesList.Count;
            isRight = true;
        }

        StartCoroutine(TentacleAttack(tentacleToHit, info));
        StartCoroutine(TentacleAttack(secondTentacleIndex, info));

        yield return new WaitForSeconds(1);

        if (isRight) {
            tentacleToHit = (tentacleToHit - 1 + _tentaclesList.Count) % _tentaclesList.Count;
            secondTentacleIndex = (secondTentacleIndex + 1) % _tentaclesList.Count;
        }
        else {
            tentacleToHit = (tentacleToHit + 1) % _tentaclesList.Count;
            secondTentacleIndex = (secondTentacleIndex - 1 + _tentaclesList.Count) % _tentaclesList.Count;
        }

        StartCoroutine(TentacleAttack(tentacleToHit, info));
        StartCoroutine(TentacleAttack(secondTentacleIndex, info));

        StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator KrakenSpinningAttackCoroutine(int skillIndex) {
        KrakenSpinningAttack info = _listOfSkills[skillIndex] as KrakenSpinningAttack;

        int tentacleToHit = FindClosestTentacleToPlayer();

        Vector3 tentaclePos = _tentaclesList[tentacleToHit].transform.position;
        Vector3 playerPos = _player.position;
        Vector3 centerPos = Vector3.zero;

        Vector3 tentacleDir = (tentaclePos - centerPos).normalized;
        Vector3 playerDir = (playerPos - centerPos).normalized;

        Vector3 cross = Vector3.Cross(tentacleDir, playerDir);

        bool isRight = cross.y < 0;

        StartCoroutine(TentacleAttack(tentacleToHit, info));
        yield return new WaitForSeconds(info.CooldownBetweenEachTentacle);

        if (!isRight) {
            for (int i = 0; i < _listOfTentacles.Count - 1; i++) {
                if (tentacleToHit == _listOfTentacles.Count - 1) tentacleToHit = -1;
                tentacleToHit++;
                StartCoroutine(TentacleAttack(tentacleToHit, info));
                yield return new WaitForSeconds(info.CooldownBetweenEachTentacle);
            }
        }
        else {
            for (int i = 0; i < _listOfTentacles.Count - 1; i++) {
                if (tentacleToHit == 0) tentacleToHit = _listOfTentacles.Count - 1; 
                tentacleToHit--;
                StartCoroutine(TentacleAttack(tentacleToHit, info));
                yield return new WaitForSeconds(info.CooldownBetweenEachTentacle);
            }
        }

        StartCoroutine(CooldownBetweenAttacks());
    }
    IEnumerator KrakenCrossAttackCoroutine(int skillIndex) {
        KrakenCrossAttack info = _listOfSkills[skillIndex] as KrakenCrossAttack;

        int tentacleToHit = FindClosestTentacleToPlayer();

        bool isPair = tentacleToHit % 2 == 0;

        if (isPair) {
            for (int i = 1; i < _listOfTentacles.Count + 1; i++) {
                if (i % 2 != 0) StartCoroutine(TentacleAttack(i - 1, info)); 
            }
        }
        else {
            for (int i = 1; i < _listOfTentacles.Count + 1; i++) {
                if (i % 2 == 0) StartCoroutine(TentacleAttack(i - 1, info));
            }
        }

        yield return new WaitForSeconds(info.CooldownBetweenAttacks);

        if (!isPair) {
            for (int i = 1; i < _listOfTentacles.Count + 1; i++) {
                if (i % 2 != 0) StartCoroutine(TentacleAttack(i - 1, info));
            }
        }
        else {
            for (int i = 1; i < _listOfTentacles.Count + 1; i++) {
                if (i % 2 == 0) StartCoroutine(TentacleAttack(i - 1, info));
            }
        }

        StartCoroutine(CooldownBetweenAttacks());
    }
    #endregion
    IEnumerator TentacleAttack(int tentacleIndex, EnemySkillSO skill) {

        Animator anim = _listOfTentacles[tentacleIndex].Anim;
        anim.SetTrigger(AttackAnimationParameter);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(AttackAnimationName));

        int attackStateHash = stateInfo.fullPathHash;

        while (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == attackStateHash &&
       anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            yield return null;
        }

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (!stateInfo.IsName(AttackHitAnimationName));

        attackStateHash = stateInfo.fullPathHash;
        SkillAnimationEvent skillEvent = skill._listOfPrefabs[0];

        do {
            yield return null;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        } while (stateInfo.fullPathHash == attackStateHash && stateInfo.normalizedTime < skillEvent.timeToSpawnHitBox);

        GameObject attackHitBox = SkillPoolingManager.Instance.ReturnHitboxFromPool(skillEvent.hitboxName, skillEvent.hitboxPrefab);
        float yRotation = 180 + (tentacleIndex * 45);
        attackHitBox.transform.SetPositionAndRotation(new Vector3(0, 3, 0), Quaternion.Euler(90, yRotation, 0));

        InstantDamageContext newContext = new(
            10,
            0.1f,
            false,
            Tags.Player
            );

        attackHitBox.GetComponent<InstantDamageHitBox>().Initialize(newContext);

        _listOfTentacles[tentacleIndex].HitBox.SetActive(true);

        yield return new WaitForSeconds(3);

        anim.SetTrigger(ReturnToIdleAnimationParameter);

        _listOfTentacles[tentacleIndex].HitBox.SetActive(false);
    }
}
