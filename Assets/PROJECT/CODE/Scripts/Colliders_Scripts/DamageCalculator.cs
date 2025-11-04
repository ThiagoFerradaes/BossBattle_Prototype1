using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCollider { Instant, Continuos, Projectile}
public enum DamageType { Abyssal, Ancestral, Pure }
public enum ExtraDamageContextAtributes { Penetration, CritRate, CritDamage }

[Serializable]
public class DamageAtributes {
    [Header ("Main Atributes")]
    public DamageType DamageType;
    public List<Tags> UnitsToHit;

    [Header("Floats")]
    public float Damage;
    bool hasDuration => TypeOfPrefab == TypeOfCollider.Instant || TypeOfPrefab == TypeOfCollider.Continuos;

    [Header("Booleans")]
    public bool HitShield = true;
    public bool BreakShield = false;

    [Header("Vectors3")]
    public Vector3 Size = Vector3.one;

    [Header("Type of Collider")]
    [SerializeField] TypeOfCollider TypeOfPrefab;

    [ShowIf("hasDuration"), AllowNesting] public float HitBoxDuration = 0.1f;

    // Continuos
    [ShowIf("TypeOfPrefab", TypeOfCollider.Continuos), AllowNesting]
    public float DamageCooldown = 0.1f;

    // Projectile
    [ShowIf("TypeOfPrefab", TypeOfCollider.Projectile), AllowNesting]
    public float Distance = 5f;
    [ShowIf("TypeOfPrefab", TypeOfCollider.Projectile), AllowNesting]
    public float Speed = 10f;
    [ShowIf("TypeOfPrefab", TypeOfCollider.Projectile), AllowNesting]
    public bool CrossEnemy = false;

    [Header("Extra atributes"), Tooltip("Penetration 0 - 0.75")]
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

        // Vendo se critou
        bool isCrit;
        if (atributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.CritRate))
            isCrit = UnityEngine.Random.value <= atributes.ExtraAtributes[ExtraDamageContextAtributes.CritRate];
        else isCrit = UnityEngine.Random.value <= statusDealer.ReturnStatusValue(StatusType.CritRate) / 100;

        // Vendo dano crítico
        if (isCrit) {
            if (atributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.CritDamage))
                rawDamage *= atributes.ExtraAtributes[ExtraDamageContextAtributes.CritDamage]/100;
            else rawDamage *= statusDealer.ReturnStatusValue(StatusType.CritDamage) / 100;
        }

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
