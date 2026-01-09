using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ LightGod")]
public class LilianLightGodSO : UltimateSkillSO {
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Gods infos")]
    [Foldout("Specific")] public Vector3 ManagerLocalPosition;
    [Foldout("Specific")] public float SelfDamageLostOverTime;
    [Foldout("Specific")] public float CooldownBetweenSelfDamage;
    [Foldout("Specific"), Tooltip("Value between 0-100")] public float PercentOfMinHealth;
    [Foldout("Specific")] public float HealthToHealBeforeUlt;
}
