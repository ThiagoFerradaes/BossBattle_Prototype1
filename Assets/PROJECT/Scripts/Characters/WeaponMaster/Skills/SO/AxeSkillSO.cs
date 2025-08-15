using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills / AxeSkill")]
public class AxeSkillSO : SkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string FirstAnimationParameterName;
    [Foldout("Specific")] public string SecondAnimationParameterName;
    [Foldout("Specific")] public string SecondAnimationName;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float Cooldown;
    [Foldout("Specific")] public float MinimalChargeTime;
    [Foldout("Specific")] public float MaxChargeTime;
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float Penetration;

    [Header("Strings")]
    [Foldout("Specific")] public string WeaponName;

    [Header("Enums")]
    [Foldout("Specific")] public Tags EnemyTag;
    [Foldout("Specific")] public DamageType DamageType;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
