using System.Collections.Generic;
using UnityEngine;


public class EnemyBehaviourSO : ScriptableObject
{
    public int Priority;
    public float Cooldown;
    protected EnemyBehaviourManager enemyBehaviourManager;
    public List<SkillAnimationEvent> Prefabs;

    public virtual void StartState(EnemyBehaviourManager parent) { 
        enemyBehaviourManager = parent; 
        parent.CooldownManager.SetSkillCooldown(this);
    }

    public virtual void UpdateState() { }

    public virtual void ExitState() { }
}
