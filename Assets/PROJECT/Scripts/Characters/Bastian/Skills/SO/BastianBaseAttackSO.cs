using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/BastianBaseAttackSO")]
public class BastianBaseAttackSO : CommonSkillSO {

    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AnimationOneParameter;
    [Foldout("Specific")] public string AnimationTwoParameter;
    [Foldout("Specific")] public string AnimationThreeParameter;

    [Header("Animation Name")]
    [Foldout("Specific")] public string AnimationOneName;
    [Foldout("Specific")] public string AnimationTwoName;
    [Foldout("Specific")] public string AnimationThreeName;

    [Header("Animation Speed")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Attacks Atributes")]
    [Foldout("Specific")] public float FirstAttackMinDamage;
    [Foldout("Specific")] public float FirstAttackMaxDamage;
    [Foldout("Specific")] public float SecondAttackMinDamage;
    [Foldout("Specific")] public float SecondAttackMaxDamage;
    [Foldout("Specific")] public float ThirdAttackMinDamage;
    [Foldout("Specific")] public float ThirdAttackMaxDamage;
    [Foldout("Specific")] public float AttackDistance;
    [Foldout("Specific")] public float ProjectileSpeed;
    [Foldout("Specific")] public bool HitShield;

    [Header("Cooldown")]
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;

    [Header("Passive")]
    [Foldout("Specific")] public float HeatGain;
    [Foldout("Specific")] public float PenetrationOnSuperHeat;
    [Foldout("Specific")] public float CritChanceOverHeat;
    [Foldout("Specific")] public float LastOverHeatCritDamage;

}
