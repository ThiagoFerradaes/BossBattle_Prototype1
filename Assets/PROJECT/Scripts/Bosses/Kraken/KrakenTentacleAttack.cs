using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / TentacleAttack")]
public class KrakenTentacleAttack : EnemyBehaviourSO
{

    [Foldout("Animations")] public string AttackAnimationParameter;
    [Foldout("Animations")] public string ReturnToIdleAnimationParameter;
    [Foldout("Animations")] public string AttackAnimationName;
    [Foldout("Animations")] public string AttackHitAnimationName;
    [Foldout("Animations")] public string ReturnToIdleAnimationName;
    [Foldout("Animations")] public string PreparingAttackSpeed;
    [Foldout("Animations")] public string HitAttackSpeed;

    [Foldout("Variables")] public float TentacleDamage;
    [Foldout("Variables")] public float DeadTentacleDamage;

}
