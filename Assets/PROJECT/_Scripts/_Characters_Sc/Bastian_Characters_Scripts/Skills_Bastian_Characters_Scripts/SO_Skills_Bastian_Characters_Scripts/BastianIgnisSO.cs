using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Ignis")]
public class BastianIgnisSO : CommonSkillSO
{
    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Passive")]
    [Foldout("Specific")] public float HeatGain;
}
