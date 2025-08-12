using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / TentacleAttack")]
public class KrakenTentacleAttack : EnemySkillSO
{
    public string AttackAnimationParameter;
    public string ReturnToIdleAnimationParameter;
    public string AttackAnimationName;
    public string AttackHitAnimationName;
    public string ReturnToIdleAnimationName;
    public float TentacleDamage;
}
