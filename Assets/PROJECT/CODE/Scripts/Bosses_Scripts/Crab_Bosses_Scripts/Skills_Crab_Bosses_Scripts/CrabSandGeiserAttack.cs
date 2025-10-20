using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ SandGeiser")]
public class CrabSandGeiserAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    StatusManager _statusManager;

    [Header("Attack Atributes")]
    [SerializeField] float minHighTidesToAttack = 1;
    [SerializeField] float amountOfAttacks = 3;
    [SerializeField] float hitboxDuration = 0.1f;
    [SerializeField] DamageAtributes damageAtributes;

    [Header("Warning Atributes")]
    [SerializeField] float amountOfWarningRepetitions;
    [SerializeField] float cooldownBetweenWarningRepetitions;
    [SerializeField] float warningTimeOn;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        _crabManager.StartCoroutine(GeiserRoutine());

    }

    public override bool MeetsCondition()
    {
        return (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide &&
            CrabArenaManager.Instance.ReturnAmountOfTideOccurence(CrabArenaState.HighTide) >= minHighTidesToAttack);
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _statusManager = _crabManager.StatusManager;
    }

    IEnumerator GeiserRoutine()
    {

        _crabManager.StartCoroutine(CooldownBetweenAttacksRoutine());

        while (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.LowTide)
        {
            for (int i = 0; i < amountOfAttacks; i++)
            {
                yield return _crabManager.StartCoroutine(WarningCoroutine());

                float warningsDuration = (amountOfWarningRepetitions * (warningTimeOn + cooldownBetweenWarningRepetitions));
                float geiserDuration = (amountOfAttacks * (warningsDuration + hitboxDuration));
                float cooldownBetweenGeisers = (CrabArenaManager.Instance.ReturnCurrentTideRemainingTime() - geiserDuration) / (amountOfAttacks + 1);
                yield return new WaitForSeconds(cooldownBetweenGeisers);
            }
            break;
        }

    }


    void InstantiateHitBoxAttack()
    {

        CrabPlatformManager platform = CrabArenaManager.Instance.CrabPlatform.GetComponent<CrabPlatformManager>();

        DamageContext context = new(
        damageAtributes,
        hitboxDuration,
        _statusManager
        );

        List<InstantDamageHitBox> hitboxes = platform.ReturnPlatformDamageCollider();
        foreach (var hitbox in hitboxes)
        {
            hitbox.Initialize(context);
        }
    }

    IEnumerator WarningCoroutine()
    {
        List<GameObject> listOfWarnings = CrabArenaManager.Instance.CrabPlatform.GetComponent<CrabPlatformManager>().ReturnPlatformWarningObject();
        for (int i = 0; i < amountOfWarningRepetitions; i++)
        {
            foreach (var warning in listOfWarnings)
            {
                warning.SetActive(true);
            }

            yield return new WaitForSeconds(warningTimeOn);

            foreach (var warning in listOfWarnings)
            {
                warning.SetActive(false);
            }

            yield return new WaitForSeconds(cooldownBetweenWarningRepetitions);
        }

        InstantiateHitBoxAttack();

    }
}
