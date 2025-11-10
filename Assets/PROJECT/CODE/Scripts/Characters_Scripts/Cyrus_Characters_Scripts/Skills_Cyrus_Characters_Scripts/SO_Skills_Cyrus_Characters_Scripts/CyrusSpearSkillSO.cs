using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ SpearAttack")]
public class CyrusSpearSkillSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string SpearAttackTriggerName;
    [Foldout("Specific")] public string AnimationName;

    [Header("Level Up Buffs")]
    [Foldout("Specific")] public float Level2Range;
    [Foldout("Specific")] public float Level3Penetration;
    [Foldout("Specific")] public float Level3Cooldown;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Strings")]
    [Foldout("Specific")] public string SpearName;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject SpearPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
