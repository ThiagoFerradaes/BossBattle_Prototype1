using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ DanceUltimate")]
public class GraciaDanceUltimateSO : UltimateSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string attackAnimationParameter;
    [Foldout("Specific")] public string attackAnimationName;

    [Header("Blue atributes")]
    [Foldout("Specific")] public DamageAtributes BlueAtributes;
    [Foldout("Specific")] public float BlueAmountOfAuraConsumed;
    [Foldout("Specific")] public List<RangeFloatOne> BlueSizeIncreasePerLevel;
    [Foldout("Specific")] public List<RangeFloatOne> BlueDamageCooldownDecreasePerLevel;

    [Header("Yellow atributes")]
    [Foldout("Specific")] public DamageAtributes YellowAtributes;
    [Foldout("Specific")] public float YellowAmountOfAuraConsumed;
    [Foldout("Specific")] public List<float> YellowDurationPerLevel;

    [Header("Red atributes")]
    [Foldout("Specific")] public DamageAtributes RedAtributes;
    [Foldout("Specific")] public float RedAmountOfAuraConsumed;
    [Foldout("Specific")] public List<RangeFloatOne> RedCriRateIncreasePerLevel;

    [Header("Green atributes")]
    [Foldout("Specific")] public DamageAtributes GreenAtributes;
    [Foldout("Specific")] public float GreenUltimateShieldAmount;
    [Foldout("Specific")] public float GreenUltimateShieldDuration;
    [Foldout("Specific")] public float GreenAmountOfAuraConsumed;
    [Foldout("Specific"), Tooltip("Low values, between 0 - 2 (1 == 100%)")] public List<float> GreenDamageCooldownDecreasePerLevel;
}
