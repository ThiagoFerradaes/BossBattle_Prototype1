using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Last Whisper")]
public class BastianLastWhisperSO : UltimateSkillSO
{
    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Heat")]
    [Foldout("Specific")] public float HeatDamageMultiplier;
    [Foldout("Specific")] public float PenetrationOnSuperHeat;
    [Foldout("Specific")] public float CritChanceOverHeat;
    [Foldout("Specific")] public float LastOverHeatCritDamage;
}
