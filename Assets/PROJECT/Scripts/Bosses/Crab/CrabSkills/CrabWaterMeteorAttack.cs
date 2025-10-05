using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Crab/ Skills/ WaterMeteor")]
public class CrabWaterMeteorAttack : EnemyBehaviourSO
{
    CrabManager _crabManager;
    Animator _anim;
    Transform _vallis;
    StatusManager _statusManager;

    [Header("Attack atributes")]
    [SerializeField] float cooldownBetweenAttacks;
    [SerializeField] float amountOfAttacks;
    [SerializeField] float cooldownBetweenMeteors;
    [SerializeField] float maxDistanceToPlayer;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        Initialize(parent);

        Debug.Log("MeteorAttack");

        _crabManager.StartCoroutine(CooldownBetweenAttacks());
    }

    public override bool MeetsCondition()
    {
        if (CrabArenaManager.Instance.ReturnCurrentTide() == CrabArenaState.HighTide) return true;

        return false;
    }

    void Initialize(EnemyBehaviourManager parent)
    {
        if (_crabManager != null) return;

        _crabManager = parent as CrabManager;
        _anim = _crabManager.Anim;
        _vallis = _crabManager.Vallis;
        _statusManager = _crabManager.StatusManager;
    }

    IEnumerator CooldownBetweenAttacks()
    {
        yield return new WaitForSeconds(cooldownBetweenAttacks);

        _crabManager.ChangeBehaviourAtRandom(Channel);
    }
}
