using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ BaseAttack")]
public class GraciaAttackSO : CommonSkillSO {
    [Header("Animations")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public DamageAtributes FirstAttackAtributes;
    [Foldout("Specific")] public DamageAtributes SecondAttackAtributes;
    [Foldout("Specific")] public DamageAtributes ThirdAttackAtributes;
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;
}
