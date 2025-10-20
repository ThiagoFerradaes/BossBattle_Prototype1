using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Cyrus/BaseAttack")]
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
    [Foldout("Specific")] public float FirstAttackHitBoxDuration;
    [Foldout("Specific")] public float SecondAttackHitBoxDuration;
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;

    [Header("Strings")]
    [Foldout("Specific")] public string SwordName;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject SwordPrefab;
    [Foldout("Specific")] public Vector3 FirstBaseAttackHitBoxPosition;
    [Foldout("Specific")] public Vector3 SecondtBaseAttackHitBoxPosition;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
