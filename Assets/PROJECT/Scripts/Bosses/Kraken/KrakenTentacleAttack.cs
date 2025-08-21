using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / TentacleAttack")]
public class KrakenTentacleAttack : EnemyBehaviourSO
{
    public string AttackAnimationParameter;
    public string ReturnToIdleAnimationParameter;
    public string AttackAnimationName;
    public string AttackHitAnimationName;
    public string ReturnToIdleAnimationName;
    public float TentacleDamage;

}
