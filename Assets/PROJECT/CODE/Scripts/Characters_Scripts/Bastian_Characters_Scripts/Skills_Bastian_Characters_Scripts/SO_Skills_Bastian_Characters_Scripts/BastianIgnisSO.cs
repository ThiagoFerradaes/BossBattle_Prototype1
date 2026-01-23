using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Ignis")]
public class BastianIgnisSO : CommonSkillSO
{
    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Passive")]
    [Foldout("Specific")] public float HeatGain;
    [Foldout("Specific")] public float PenetrationOnSuperHeat;
    [Foldout("Specific")] public float CritChanceOverHeat;
    [Foldout("Specific")] public float LastOverHeatCritDamage;

}
