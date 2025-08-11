using UnityEngine;

public enum DamageType { Physical, Magic, True}
public static class DamageCalculator
{
    public static float CalculateDamage(
        DamageType skillType, 
        float skillBaseDamage, 
        float penetration,
        StatusManager statusDealer,
        StatusManager statusReciever
        ) {

        float rawDamage = skillType switch {
            DamageType.Physical => skillBaseDamage * statusDealer.ReturnStatusValue(StatusType.BaseAttack),
            DamageType.Magic => skillBaseDamage * statusDealer.ReturnStatusValue(StatusType.SkillAttack),
            DamageType.True => skillBaseDamage,
            _ => skillBaseDamage
        };

        bool isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate);
        if (isCrit) rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage);

        float targetDefense = skillType switch {
            DamageType.Physical => statusReciever.ReturnStatusValue(StatusType.Defense),
            DamageType.Magic => statusReciever.ReturnStatusValue(StatusType.SkillDefense),
            DamageType.True => 0,
            _ => 0
        };

        targetDefense *= (1 - penetration);

        float finalDamage = rawDamage * (100/ (100 + targetDefense));

        return Mathf.Max(1, finalDamage);
    }
}
