using System;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Abyssal, Ancestral, Pure }
[Serializable]
public class DamageAtributes
{
    public float Damage;
    [Range(0, 1)] public float Penetration;
    public bool HitShield;
    public DamageType DamageType;
    public List<Tags> UnitsToHit;
}
public static class DamageCalculator
{
    public static (float, bool) CalculateDamage( // Considerando o crítico do personagem
        DamageAtributes atributes,
        StatusManager statusDealer,
        StatusManager statusReciever
        )
    {

        float rawDamage = atributes.DamageType switch
        {
            DamageType.Abyssal => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => atributes.Damage,
            _ => atributes.Damage
        };

        bool isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate) / 100;
        if (isCrit) rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage) / 100;

        float targetDefense = atributes.DamageType switch
        {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        float penetration = Mathf.Min(0.75f, atributes.Penetration);

        targetDefense *= (1 - penetration);

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return (Mathf.Max(1, finalDamage), isCrit);
    }

    public static (float, bool) CalculateDamage( // Considerando um crítico a parte
    DamageAtributes atributes,
    float critRate,
    float critDamage,
    StatusManager statusDealer,
    StatusManager statusReciever
    )
    {

        float rawDamage = atributes.DamageType switch
        {
            DamageType.Abyssal => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => atributes.Damage,
            _ => atributes.Damage
        };

        bool isCrit = UnityEngine.Random.value <= critRate / 100;
        if (isCrit) rawDamage *= critDamage / 100;

        float targetDefense = atributes.DamageType switch
        {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        float penetration = Mathf.Min(0.75f, atributes.Penetration);

        targetDefense *= (1 - penetration);

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return (Mathf.Max(1, finalDamage), isCrit);
    }
}
