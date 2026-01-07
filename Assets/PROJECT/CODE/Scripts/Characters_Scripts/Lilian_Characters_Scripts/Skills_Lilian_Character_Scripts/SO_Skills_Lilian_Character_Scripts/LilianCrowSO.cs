using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ Crow")]

public class LilianCrowSO : CommonSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Explosion's Values")]
    [Foldout("Specific")] public DamageAtributes ExplosionAtributes;
    [Foldout("Specific")] public GameObject ExplosionPrefab;
    [Foldout("Specific")] public float ExplosionMaxDamage;
    [Foldout("Specific")] public float SkillMinCooldown;
    [Foldout("Specific")] public float SkillPercentDamageToLillian;
    [Foldout("Specific"), Tooltip("Value between 0 - 100")] public float SkillMinCooldownHealthPercent;
}
