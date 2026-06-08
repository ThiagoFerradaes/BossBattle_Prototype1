using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ FlameEcho")]
public class BastianFlameEchoSO : UltimateSkillSO
{
    [Header("Secondary Attack Damage")]
    [Foldout("Specific")] public DamageAtributes FirstAttackDamageAtributes;
    [Foldout("Specific")] public DamageAtributes SecondAttackDamageAtributes;
    [Foldout("Specific")] public DamageAtributes ThirdAttackDamageAtributes;
    [Foldout("Specific")] public float ProjectileSize;

    [Header("Secondary Attack Passive")]
    [Foldout("Specific")] public float SHeatGain;
    [Foldout("Specific")] public float SHeatGainOverHeat;
    [Foldout("Specific")] public float SPenetrationOnSuperHeat;
    [Foldout("Specific")] public float SCritChanceOverHeat;
    [Foldout("Specific")] public float SLastOverHeatCritDamage;

    [Header("Secondary Ignis")]
    [Foldout("Specific")] public DamageAtributes IgnisDamageAtributes;
    [Foldout("Specific")] public float IgnisHeatGain;
    [Foldout("Specific")] public float IgnisHeatGainOverHeat;
    [Foldout("Specific")] public float IgnisPenetrationOnSuperHeat;
    [Foldout("Specific")] public float IgnisCritChanceOverHeat;
    [Foldout("Specific")] public float IgnisLastOverHeatCritDamage;
    [Foldout("Specific")] public float TimeBetweenIgnis;

    [Header("Ultimate Atributes")]
    [Foldout("Specific")] public float UltimateDuration;
    [Foldout("Specific")] public float TimeBetweenFirstAndSecondShoot;
}
