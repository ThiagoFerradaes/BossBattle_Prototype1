using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ DashUltimate")]
public class GraciaDashUltimateSO : UltimateSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string AttackAnimationParameter;
    [Foldout("Specific")] public string AttackAnimationName;

    [Header("Dash Atributes")]
    [Foldout("Specific")] public float dashForce;
}
