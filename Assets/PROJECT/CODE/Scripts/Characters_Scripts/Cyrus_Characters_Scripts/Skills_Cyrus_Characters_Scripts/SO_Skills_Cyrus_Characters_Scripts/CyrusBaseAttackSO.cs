using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ BaseAttack")]
public class CyrusBaseAttackSO : CommonSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string FirstBaseAttackParameter;
    [Foldout("Specific")] public string FirstBaseAttackAnimationName;
    [Foldout("Specific")] public string SecondBaseAttackParameter;
    [Foldout("Specific")] public string SecondtBaseAttackAnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public DamageAtributes FirstAttackAtributes;
    [Foldout("Specific")] public DamageAtributes SecondAttackAtributes;
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;
    [Foldout("Specific")] public Vector3 Size;
}
