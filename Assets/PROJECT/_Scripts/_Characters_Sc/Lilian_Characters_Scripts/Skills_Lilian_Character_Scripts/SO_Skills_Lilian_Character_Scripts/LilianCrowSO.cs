using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ Crow")]

public class LilianCrowSO : CommonSkillSO {

    [Header("Explosion's Values")]
    [Foldout("Specific")] public DamageAtributes ExplosionAtributes;
    [Foldout("Specific")] public float ExplosionMaxDamage;
    [Foldout("Specific")] public float SkillMinCooldown;
    [Foldout("Specific"), Range(0,100)] public float SkillPercentDamageToLillian;
    [Foldout("Specific")] public float ExplosionEnergy;
    [Foldout("Specific"), Range(0,100)] public float SkillMinCooldownHealthPercent;
}
