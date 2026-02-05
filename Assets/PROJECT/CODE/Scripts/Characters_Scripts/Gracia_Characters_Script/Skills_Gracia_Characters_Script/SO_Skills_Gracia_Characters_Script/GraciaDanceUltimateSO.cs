using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ DanceUltimate")]
public class GraciaDanceUltimateSO : UltimateSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string attackAnimationParameter;
    [Foldout("Specific")] public string attackAnimationName;
}
