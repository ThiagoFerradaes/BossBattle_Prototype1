using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ JudgmentDay")]
public class LilianJudgmentDaySO : UltimateSkillSO
{

    [Header("Atributes")]
    [Foldout("Specific")] public float DamageToLilian;
    [Foldout("Specific")] public float DamageCooldownToLilian;
    [Foldout("Specific")] public float HealthLimit;
    [Foldout("Specific")] public float InitialHeal;
}
