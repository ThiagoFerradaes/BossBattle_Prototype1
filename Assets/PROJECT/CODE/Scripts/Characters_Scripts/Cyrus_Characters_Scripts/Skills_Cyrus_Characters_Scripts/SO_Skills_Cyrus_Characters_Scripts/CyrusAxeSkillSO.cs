using AYellowpaper.SerializedCollections;
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

    [Header("Level 1 & 2 Buffs")]
    [Foldout("Specific")] public float Level1AmountOfShield;
    [Foldout("Specific")] public float Level2AmountOfShield;
    [Foldout("Specific")] public float ShieldDuration;
    [Foldout("Specific")] public float NewMaxChargeTime;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Level 3 Buff")]
    [Foldout("Specific")] public DamageAtributes RocksAtributes;
    [Foldout("Specific")] public float BrokenRockDamageCooldown;
    [Foldout("Specific")] public float BrokenRockDuration;
    [Foldout("Specific")] public float BrokenRockSize;
    [Foldout("Specific")] public string BrokenRocksName;
    [Foldout("Specific")] public GameObject BrokenRocksPrefab;

    [Header("Weapon")]
    [Foldout("Specific")] public string WeaponName;
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
