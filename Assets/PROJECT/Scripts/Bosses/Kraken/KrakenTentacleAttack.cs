using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / TentacleAttack")]
public class KrakenTentacleAttack : EnemyBehaviourSO {
    [Foldout("Prefabs")] public List<SkillAnimationEvent> PrefabsPreparingAnimation;
    [Foldout("Prefabs")] public List<SkillAnimationEvent> PrefabsHitAnimation;

    [Foldout("Animations")] public string AttackAnimationParameter;
    [Foldout("Animations")] public string ReturnToIdleAnimationParameter;
    [Foldout("Animations")] public string AttackAnimationName;
    [Foldout("Animations")] public string AttackHitAnimationName;
    [Foldout("Animations")] public string ReturnToIdleAnimationName;
    [Foldout("Animations")] public string PreparingAttackSpeed;
    [Foldout("Animations")] public string HitAttackSpeed;
    [Foldout("Animations")] public float TimeInReturnToIdleToTurnOffHitBox;

    [Foldout("Variables")] public float TentacleDamage;
    [Foldout("Variables")] public float DeadTentacleDamage;
    [Foldout("Variables"), Range(0, 100)] public float Penetration;
    [Foldout("Variables"), Range(0, 1)] public float TentacleAttackSize;
    [Foldout("Variables")] public bool HitShield;
    [Foldout("Variables")] public DamageType DamageType;
    [Foldout("Variables")] public List<Tags> Tags;

}
