using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Abyssal, Ancestral, Pure }
[Serializable]
public class DamageAtributes {
    [Header("Floats")]
    public float Damage;
    public float HitBoxDuration = 0.1f;

    [Header("Booleans")]
    public bool HitShield = true;
    public bool BreakShield = false;
    public bool CrossEnemy = false;

    [Header("Other variables")]
    public DamageType DamageType;
    public List<Tags> UnitsToHit;
    [SerializedDictionary("Extra atribute", "Value")]
    public SerializedDictionary<ExtraDamageContextAtributes, float> ExtraAtributes;
}
public static class DamageCalculator {
    public static (float, bool) CalculateDamage( // Considerando o crítico do personagem
        DamageAtributes atributes,
        StatusManager statusDealer,
        StatusManager statusReciever
        ) {

        float rawDamage = atributes.DamageType switch {
            DamageType.Abyssal => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => atributes.Damage,
            _ => atributes.Damage
        };

        bool isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate) / 100;
        if (isCrit) rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage) / 100;

        float targetDefense = atributes.DamageType switch {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        float penetration = 0;
        
        if (atributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.Penetration))
            penetration = Mathf.Min(0.75f, atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration]);

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
    ) {

        float rawDamage = atributes.DamageType switch {
            DamageType.Abyssal => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => atributes.Damage * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => atributes.Damage,
            _ => atributes.Damage
        };

        bool isCrit = UnityEngine.Random.value <= critRate / 100;
        if (isCrit) rawDamage *= critDamage / 100;

        float targetDefense = atributes.DamageType switch {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        float penetration = 0;

        if (atributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.Penetration))
            penetration = Mathf.Min(0.75f, atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration]);

        targetDefense *= (1 - penetration);

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return (Mathf.Max(1, finalDamage), isCrit);
    }
}
