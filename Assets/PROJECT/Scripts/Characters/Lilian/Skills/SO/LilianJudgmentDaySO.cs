using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Lilian/JudgmentDay")]
public class LilianJudgmentDaySO : UltimateSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Values")]
    [Foldout("Specific"), Range(0, 100)] public float PercentOfCurrentHealthToCauseWrath;
}
