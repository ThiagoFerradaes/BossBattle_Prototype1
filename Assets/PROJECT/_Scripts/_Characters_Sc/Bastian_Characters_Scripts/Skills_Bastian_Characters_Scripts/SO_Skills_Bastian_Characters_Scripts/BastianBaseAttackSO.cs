using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ BastianBaseAttackSO")]
public class BastianBaseAttackSO : CommonSkillSO {

    [Header("Animation Speed")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Attacks Atributes")]
    [Foldout("Specific")] public DamageAtributes FirstAttackAtributes;
    [Foldout("Specific")] public DamageAtributes SecondAttackAtributes;
    [Foldout("Specific")] public DamageAtributes ThirdAttackAtributes;
    [Foldout("Specific")] public float AttackDistance;
    [Foldout("Specific")] public float ProjectileSpeed;
    [Foldout("Specific")] public float ProjectileSize = 0.5f;
    [Foldout("Specific")] public List<AK.Wwise.Switch> Switchs;

    [Header("Cooldown")]
    [Foldout("Specific")] public float CooldownBetweenAttacks;
    [Foldout("Specific")] public float MaxTimeBetweenAttacks;

    [Header("Passive")]
    [Foldout("Specific")] public float HeatGain;
}
