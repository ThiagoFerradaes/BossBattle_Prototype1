using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "Bosses/ Behaviour/ Generic/ InitialCooldown")]
public class BossInitialAttack : EnemyBehaviourSO {

    [SerializeField] float cooldown;

    public override void StartState(EnemyBehaviourManager parent)
    {
        base.StartState(parent);

        parent.StartCoroutine(CooldownTimer());
    }
    IEnumerator CooldownTimer()
    {
        float timer = 0;
        while ( timer < cooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        enemyBehaviourManager.OpenChannel(Channel);
        enemyBehaviourManager.ChangeBehaviourAtRandom();
    }
}
