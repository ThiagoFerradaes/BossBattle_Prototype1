using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Characters/ Skills / Cyrus/ ShootUpUltimate")]
public class CyrusShootUpSO : UltimateSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameterTrigger;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string LastAnimationName;

    [Header("Level Up bufs")]
    [Foldout("Specific")] public float Duration;
    [Foldout("Specific")] public float Level1Duration;
    [Foldout("Specific")] public float AditionalCritRate;
    [Foldout("Specific")] public float AditionalCritDamagePerHit;
    [Foldout("Specific")] public float Level3DamageCooldown;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponTwoPosition;
    [Foldout("Specific")] public Vector3 WeaponOneRotation;
    [Foldout("Specific")] public Vector3 WeaponTwoRotation;
}
