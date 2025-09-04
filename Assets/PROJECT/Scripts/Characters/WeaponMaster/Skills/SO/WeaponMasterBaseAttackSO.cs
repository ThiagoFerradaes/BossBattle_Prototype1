using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills / BaseAttack")]
public class WeaponMasterBaseAttackSO : CommonSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string FirstBaseAttackParameter;
    [Foldout("Specific")] public string FirstBaseAttackAnimationName;
    [Foldout("Specific")] public string SecondBaseAttackParameter;
    [Foldout("Specific")] public string SecondtBaseAttackAnimationName;
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")]public float FirstAttackMinDamage;
    [Foldout("Specific")]public float FirstAttackMaxDamage;
    [Foldout("Specific")] public float SecondAttackMinDamage;
    [Foldout("Specific")] public float SecondAttackMaxDamage;
    [Foldout("Specific")] public float FirstAttackHitBoxDuration;
    [Foldout("Specific")] public float SecondAttackHitBoxDuration;
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;
    [Foldout("Specific")] public float PenetrationFirstAttack;
    [Foldout("Specific")] public float PenetrationSecondAttack;

    [Header("Strings")]
    [Foldout("Specific")] public string SwordName;

    [Header("Booleans")]
    [Foldout("Specific")] public bool HitShield;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject SwordPrefab;
    [Foldout("Specific")] public Vector3 FirstBaseAttackHitBoxPosition;
    [Foldout("Specific")] public Vector3 SecondtBaseAttackHitBoxPosition;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
