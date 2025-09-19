using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Skills/Cyrus/ShootUpUltimate")]
public class CyrusShootUpSO : UltimateSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameterTrigger;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string LastAnimationName;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float Duration;
    [Foldout("Specific")] public float DamageCooldown;
    [Foldout("Specific")] public float Size;

    [Header("Booleans")]
    [Foldout("Specific")] public bool HitShield;

    [Header("Strings")]
    [Foldout("Specific")] public string WeaponName;

    [Header("Level Up bufs")]
    [Foldout("Specific")] public float Level1Duration;
    [Foldout("Specific")] public float AditionalCritRate;
    [Foldout("Specific")] public float AditionalCritDamagePerHit;
    [Foldout("Specific")] public float Level3DamageCooldown;

    [Header("Cost")]
    [Foldout("Specific"), Range(1,100)] public float PercentOfSlow;
    [Foldout("Specific")] public float SlowDuration;

    [Header("Enums")]
    [Foldout("Specific")] public List<Tags> EnemyTag;
    [Foldout("Specific")] public DamageType DamageType;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponTwoPosition;
    [Foldout("Specific")] public Vector3 WeaponOneRotation;
    [Foldout("Specific")] public Vector3 WeaponTwoRotation;
}
