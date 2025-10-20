using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/FlameEcho")]
public class BastianFlameEchoSO : UltimateSkillSO
{
    [Header("Secondary Attack Damage")]
    [Foldout("Specific")] public DamageAtributes FirstAttackDamageAtributes;
    [Foldout("Specific")] public DamageAtributes SecondAttackDamageAtributes;
    [Foldout("Specific")] public DamageAtributes ThirdAttackDamageAtributes;
    [Foldout("Specific")] public float ProjectileSpeed;
    [Foldout("Specific")] public float AttackDistance;

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
}
