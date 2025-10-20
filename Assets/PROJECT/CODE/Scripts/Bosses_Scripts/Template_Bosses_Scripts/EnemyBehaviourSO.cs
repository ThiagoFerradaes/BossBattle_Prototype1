using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBehaviourSO : ScriptableObject
{
    #region Parameters

    public int Priority;
    public float Cooldown;
    public float CooldownBetweenAttacks;
    protected EnemyBehaviourManager enemyBehaviourManager;

    [Header("Channels")]
    public int Channel = 0;
    public bool OpenNewChannels = false;
    [ShowIf("OpenNewChannels")] public List<int> ListOfChannelsToOpen = new();
    public bool CloseOldChannels = false;
    [ShowIf("CloseOldChannels")] public List<int> ListOfChannelsToClose = new();

    #endregion

    public virtual bool MeetsCondition() => true;

    public virtual void StartState(EnemyBehaviourManager parent) { 
        enemyBehaviourManager = parent; 
        parent.CooldownManager.SetSkillCooldown(this);

        if (CloseOldChannels) {
            foreach (var channel in ListOfChannelsToClose) {
                enemyBehaviourManager.CloseChannel(channel);
            }
        }
    }

    public virtual void ExitState() { }

    public virtual IEnumerator CooldownBetweenAttacksRoutine() {
        enemyBehaviourManager.DesactivateChannel(Channel);
        yield return new WaitForSeconds(CooldownBetweenAttacks);
         
        if (OpenNewChannels) {
            foreach (var channel in ListOfChannelsToOpen) {
                enemyBehaviourManager.OpenChannel(channel);
                enemyBehaviourManager.ChangeBehaviourAtRandom(channel);
            }
        }
        else enemyBehaviourManager.ChangeBehaviourAtRandom(Channel);
    }
}
