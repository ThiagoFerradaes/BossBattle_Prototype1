using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/FlameEcho")]
public class BastianFlameEchoSO : UltimateSkillSO
{
    [Header("Secondary Attack Damage")]
    [Foldout("Specific")] public float SFirstAttackMinDamage;
    [Foldout("Specific")] public float SFirstAttackMaxDamage;
    [Foldout("Specific")] public float SSecondAttackMinDamage;
    [Foldout("Specific")] public float SSecondAttackMaxDamage;
    [Foldout("Specific")] public float SThirdAttackMinDamage;
    [Foldout("Specific")] public float SThirdAttackMaxDamage;
    [Foldout("Specific")] public float ProjectileSpeed;
    [Foldout("Specific")] public float AttackDistance;
    [Foldout("Specific")] public bool HitShield;
    [Foldout("Specific")] public DamageType SDamageType;

    [Header("Secondary Attack Passive")]
    [Foldout("Specific")] public float SHeatGain;
    [Foldout("Specific")] public float SPenetrationOnSuperHeat;
    [Foldout("Specific")] public float SCritChanceOverHeat;
    [Foldout("Specific")] public float SLastOverHeatCritDamage;

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Ultimate Atributes")]
    [Foldout("Specific")] public float UltimateDuration;
    [Foldout("Specific")] public float TimeBetweenFirstAndSecondShoot;
    [Foldout("Specific")] public List<Tags> EnemyTag;
}
