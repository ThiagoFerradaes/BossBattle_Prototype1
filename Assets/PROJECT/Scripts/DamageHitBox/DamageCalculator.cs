using UnityEngine;

public enum DamageType { Physical, Magic, True}
public static class DamageCalculator
{
    public static float CalculateDamage( // Considerando o crítico do personagem
        DamageType skillType, 
        float skillBaseDamage, 
        float penetration,
        StatusManager statusDealer,
        StatusManager statusReciever
        ) {

        float rawDamage = skillType switch {
            DamageType.Physical => skillBaseDamage/100 * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Magic => skillBaseDamage/100 * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.True => skillBaseDamage,
            _ => skillBaseDamage
        };

        bool isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate)/100;
        if (isCrit) rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage)/100;

        float targetDefense = skillType switch {
            DamageType.Physical => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Magic => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.True => 0,
            _ => 0
        };

        penetration = Mathf.Min(75f, penetration);

        targetDefense *= (1 - (penetration/100));

        float finalDamage = rawDamage * (100/ (100 + targetDefense));

        return Mathf.Max(1, finalDamage);
    }

    public static float CalculateDamage( // Considerando um crítico a parte
    DamageType skillType,
    float skillBaseDamage,
    float penetration,
    float critRate,
    float critDamage,
    StatusManager statusDealer,
    StatusManager statusReciever
    ) {

        float rawDamage = skillType switch {
            DamageType.Physical => skillBaseDamage / 100 * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Magic => skillBaseDamage / 100 * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.True => skillBaseDamage,
            _ => skillBaseDamage
        };

        bool isCrit = UnityEngine.Random.value <= critRate / 100;
        if (isCrit) rawDamage *= critDamage / 100;

        float targetDefense = skillType switch {
            DamageType.Physical => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Magic => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.True => 0,
            _ => 0
        };

        penetration = Mathf.Min(75f, penetration);

        targetDefense *= (1 - (penetration / 100));

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return Mathf.Max(1, finalDamage);
    }
}
