using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills / SpearAttack")]
public class SpearSkillSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string SpearAttackTriggerName;
    [Foldout("Specific")] public string AnimationName;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float HitBoxDuration;
    [Foldout("Specific")] public float Penetration;

    [Header("Booleans")]
    [Foldout("Specific")] public bool HitShield;

    [Header("Strings")]
    [Foldout("Specific")] public string SpearName;

    [Header("Enums")]
    [Foldout("Specific")] public List<Tags> EnemyTag;
    [Foldout("Specific")] public DamageType DamageType;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject SpearPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponRotation;
}
