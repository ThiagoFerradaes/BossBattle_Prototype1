using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ LightGod")]
public class LilianLightGodSO : UltimateSkillSO {
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string BeamAnimationParameter;
    [Foldout("Specific")] public string BeamAnimationName;

    [Header("Gods infos")]
    [Foldout("Specific")] public Vector3 ManagerLocalPosition;
    [Foldout("Specific")] public float SelfDamageLostOverTime;
    [Foldout("Specific")] public float CooldownBetweenSelfDamage;
    [Foldout("Specific"), Tooltip("Value between 0-100")] public float PercentOfMinHealth;
    [Foldout("Specific")] public float HealthToHealBeforeUlt;

    [Header("Beam Values")]
    [Foldout("Specific")] public float BeamSizeMultiplierByAmountOfGods;
    [Foldout("Specific")] public float BeamDamageMultiplierByAmountOfGods;
    [Foldout("Specific")] public float BeamDamageCooldownByAmountOfGods;
}
