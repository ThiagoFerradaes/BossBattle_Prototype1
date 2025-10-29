using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ WingsOhHorror")]
public class LilianWingsOfHorrorSO : CommonSkillSO
{
    [Header("Lilian Animation")]
    [Foldout("Specific")]public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Wings of Horror Animation")]
    [Foldout("Specific")] public string WingsOfHorrorAnimationParameter;
    [Foldout("Specific")] public string WingsOfHorrorAnimationName;

    [Header("Paramters")]
    [Foldout("Specific")] public float TributeCost;
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float WingsOfHorrorCooldown;
    [Foldout("Specific")] public float WingsOfHorrorDistance;
    [Foldout("Specific")] public float WingsOfHorrorHeight;
    [Foldout("Specific")] public float WingsOfHorrorDamageSize;
}
