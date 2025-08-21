using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "Boss / InitialCooldown")]
public class BossInitialAttack : EnemyBehaviourSO {

    [SerializeField] float cooldown;
    float timer = 0;
    public override void UpdateState() {
        timer += Time.deltaTime;

        if (timer >= cooldown) {
            enemyBehaviourManager.ChangeBehaviourAtRandom();
        }
    }

    public override void ExitState() {
        timer = 0;
    }
}
