using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Cyrus/AxeSkill")]
public class CyrusAxeSkillSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string FirstAnimationParameterName;
    [Foldout("Specific")] public string SecondAnimationParameterName;
    [Foldout("Specific")] public string SecondAnimationName;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float MinimalChargeTime;
    [Foldout("Specific")] public float MaxChargeTime;
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float Penetration;
    [Foldout("Specific")] public float AmountOfExpGain;

    [Header("Strings")]
    [Foldout("Specific")] public string WeaponName;

    [Header("Level Up Buffs")]
    [Foldout("Specific")] public float AmountOfShield;
    [Foldout("Specific")] public float ShieldDuration;
    [Foldout("Specific")] public float NewMaxChargeTime;
    [Foldout("Specific")] public GameObject BrokenRocksPrefab;
    [Foldout("Specific")] public string BrokenRocksName;
    [Foldout("Specific")] public float BrokenRockMinDamage;
    [Foldout("Specific")] public float BrokenRockMaxDamage;
    [Foldout("Specific")] public float BrokenRockDamageCooldown;
    [Foldout("Specific")] public float BrokenRockDuration;
    [Foldout("Specific")] public DamageType BrokenRockDamageType;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
