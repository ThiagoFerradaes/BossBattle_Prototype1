using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ LilianBaseAttack")]
public class LilianBaseAttackSO : CommonSkillSO
{

    [Header("Animation")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Atack Atributes")]
    [Foldout("Specific"), Range(0, 100)] public float DamagePercentToDamageLilian;
}
