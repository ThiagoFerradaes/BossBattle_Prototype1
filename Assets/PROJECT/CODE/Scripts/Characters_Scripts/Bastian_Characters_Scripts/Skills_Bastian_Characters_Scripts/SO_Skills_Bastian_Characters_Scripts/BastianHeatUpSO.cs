using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ HeatUp")]
public class BastianHeatUpSO : CommonSkillSO
{
    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Heat")]
    [Foldout("Specific")] public float AmountOfHeatToSetUp;
    [Foldout("Specific")] public float ExtraAmountOfHeat;
}
