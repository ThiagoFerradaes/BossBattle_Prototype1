using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Release")]
public class BastianReleaseSO : CommonSkillSO {

    [Header("Atributes")]
    [Foldout("Specific")] public float HeatLost;
    [Foldout("Specific"), Range(0, 1)] public float AttackSpeedGain;
    [Foldout("Specific")] public float AttackSpeedDuration;

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;
}
