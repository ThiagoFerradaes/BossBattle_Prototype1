using UnityEngine;

public enum DamageType { Abyssal, Ancestral, Pure}
public static class DamageCalculator
{
    public static (float,bool) CalculateDamage( // Considerando o crítico do personagem
        DamageType skillType, 
        float skillBaseDamage, 
        float penetration,
        StatusManager statusDealer,
        StatusManager statusReciever
        ) {

        float rawDamage = skillType switch {
            DamageType.Abyssal => skillBaseDamage/100 * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => skillBaseDamage/100 * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => skillBaseDamage,
            _ => skillBaseDamage
        };

        bool isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate)/100;
        if (isCrit) rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage)/100;

        float targetDefense = skillType switch {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        penetration = Mathf.Min(75f, penetration);

        targetDefense *= (1 - (penetration/100));

        float finalDamage = rawDamage * (100/ (100 + targetDefense));

        return (Mathf.Max(1, finalDamage), isCrit);
    }

    public static (float, bool) CalculateDamage( // Considerando um crítico a parte
    DamageType skillType,
    float skillBaseDamage,
    float penetration,
    float critRate,
    float critDamage,
    StatusManager statusDealer,
    StatusManager statusReciever
    ) {

        float rawDamage = skillType switch {
            DamageType.Abyssal => skillBaseDamage / 100 * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Ancestral => skillBaseDamage / 100 * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.Pure => skillBaseDamage,
            _ => skillBaseDamage
        };

        bool isCrit = UnityEngine.Random.value <= critRate / 100;
        if (isCrit) rawDamage *= critDamage / 100;

        float targetDefense = skillType switch {
            DamageType.Abyssal => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Ancestral => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.Pure => 0,
            _ => 0
        };

        penetration = Mathf.Min(75f, penetration);

        targetDefense *= (1 - (penetration / 100));

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return (Mathf.Max(1, finalDamage), isCrit);
    }
}
