using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ WingsOhHorror")]
public class LilianWingsOfHorrorSO : CommonSkillSO
{
    [Header("Lilian Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Wings of Horror Animation")]
    [Foldout("Specific")] public string WingsOfHorrorAnimationParameter;
    [Foldout("Specific")] public string WingsOfHorrorAnimationName;

    [Header("Paramters")]
    [Foldout("Specific")] public float RadiusOfAttack;
    [Foldout("Specific")] public float RotationSpeed;
    [Foldout("Specific"), Range(0, 100)] public float HealthPercentLostPerAttack;
}
