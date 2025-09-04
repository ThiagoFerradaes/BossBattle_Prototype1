using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/BastianBaseAttackSO")]
public class BastianBaseAttackSO : CommonSkillSO {

    public string AnimationOneParameter;
    public string AnimationTwoParameter;
    public string AnimationThreeParameter;

    public string AnimationOneName;
    public string AnimationTwoName;
    public string AnimationThreeName;

    public string AttackSpeedAnimationParameter;

    public float FirstAttackMinDamage, FirstAttackMaxDamage;
    public float SecondAttackMinDamage, SecondAttackMaxDamage;
    public float ThirdAttackMinDamage, ThirdAttackMaxDamage;
    public float AttackDistance, AttackSpeed;
    public bool HitShield;

    public float CooldownBetweenAttacks;
    public float MaxTimeBetweenAttacks;

    public float HeatGain;

}
