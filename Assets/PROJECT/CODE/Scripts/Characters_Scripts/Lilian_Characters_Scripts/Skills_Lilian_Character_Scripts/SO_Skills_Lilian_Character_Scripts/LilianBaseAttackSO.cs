using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ LilianBaseAttack")]
public class LilianBaseAttackSO : CommonSkillSO
{

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationOneParameter;
    [Foldout("Specific")] public string AnimationOneName;
    [Foldout("Specific")] public string AnimationTwoParameter;
    [Foldout("Specific")] public string AnimationTwoName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Atack Atributes")]
    [Foldout("Specific"), Range(0, 100)] public float DamagePercentToDamageLilian;
}
