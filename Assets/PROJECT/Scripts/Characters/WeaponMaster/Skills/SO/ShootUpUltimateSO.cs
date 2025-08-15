using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu( menuName = "Skills / ShootUpUltimate")]
public class ShootUpUltimateSO : SkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameterTrigger;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string LastAnimationName;

    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float Cooldown;
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public float DamageCooldown;
    [Foldout("Specific")] public float Penetration;

    [Header("Booleans")]
    [Foldout("Specific")] public bool HitShield;

    [Header("Strings")]
    [Foldout("Specific")] public string WeaponName;

    [Header("Enums")]
    [Foldout("Specific")] public Tags EnemyTag;
    [Foldout("Specific")] public DamageType DamageType;

    [Header("Weapon")]
    [Foldout("Specific")] public GameObject WeaponPrefab;
    [Foldout("Specific")] public Vector3 WeaponPosition;
    [Foldout("Specific")] public Vector3 WeaponTwoPosition;
    [Foldout("Specific")] public Vector3 WeaponOneRotation;
    [Foldout("Specific")] public Vector3 WeaponTwoRotation;
}
