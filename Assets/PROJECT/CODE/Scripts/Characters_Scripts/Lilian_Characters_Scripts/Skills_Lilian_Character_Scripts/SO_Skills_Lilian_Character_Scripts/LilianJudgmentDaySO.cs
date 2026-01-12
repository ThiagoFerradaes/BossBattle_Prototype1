using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ JudgmentDay")]
public class LilianJudgmentDaySO : UltimateSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Atributes")]
    [Foldout("Specific")] public float DamageToLilian;
    [Foldout("Specific")] public float DamageCooldownToLilian;
    [Foldout("Specific")] public float HealthLimit;
    [Foldout("Specific")] public float InitialHeal;
}
