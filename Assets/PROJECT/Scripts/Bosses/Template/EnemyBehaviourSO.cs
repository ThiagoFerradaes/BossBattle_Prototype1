using System.Collections;
using UnityEngine;


public class EnemyBehaviourSO : ScriptableObject
{
    public int Priority;
    public int Channel = 0;
    public float Cooldown;
    public float CooldownBetweenAttacks;
    protected EnemyBehaviourManager enemyBehaviourManager;

    public virtual bool MeetsCondition() => true;

    public virtual void StartState(EnemyBehaviourManager parent) { 
        enemyBehaviourManager = parent; 
        parent.CooldownManager.SetSkillCooldown(this);
    }

    public virtual void ExitState() { }

    public virtual IEnumerator CooldownBetweenAttacksRoutine() {
        enemyBehaviourManager.DesactivateChannel(Channel);
        yield return new WaitForSeconds(CooldownBetweenAttacks);
        enemyBehaviourManager.ChangeBehaviourAtRandom(Channel);
    }
}
