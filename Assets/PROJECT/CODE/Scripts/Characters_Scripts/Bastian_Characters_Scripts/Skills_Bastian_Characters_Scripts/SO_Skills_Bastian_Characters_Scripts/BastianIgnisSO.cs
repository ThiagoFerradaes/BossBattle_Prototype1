using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/Ignis")]
public class BastianIgnisSO : CommonSkillSO
{
    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Attacks Atributes")]
    [Foldout("Specific")] public Vector3 Size = new(1, 1, 1);

    [Header("Passive")]
    [Foldout("Specific")] public float HeatGain;
    [Foldout("Specific")] public float PenetrationOnSuperHeat;
    [Foldout("Specific")] public float CritChanceOverHeat;
    [Foldout("Specific")] public float LastOverHeatCritDamage;

}
