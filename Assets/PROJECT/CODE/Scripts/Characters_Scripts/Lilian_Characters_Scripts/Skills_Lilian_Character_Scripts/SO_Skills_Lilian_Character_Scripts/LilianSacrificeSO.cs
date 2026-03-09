using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ Sacrifice")]
public class LilianSacrificeSO : CommonSkillSO
{

    [Header("Values")]
    [Foldout("Specific"),Range(0, 100)] public float PercentOfCurrentHealthToLoose;
    [Foldout("Specific")] public float2 AmountOfShieldGainBasedOnHealth;
    [Foldout("Specific")] public float HealthLimit;
    [Foldout("Specific")] public float ShieldDuration;
}
