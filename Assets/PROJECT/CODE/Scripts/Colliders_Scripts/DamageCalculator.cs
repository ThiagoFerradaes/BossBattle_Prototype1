using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCollider { Instant, Continuos, Projectile, Boomerang}
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

    bool projectileOrBoomerang => TypeOfPrefab == TypeOfCollider.Projectile || TypeOfPrefab == TypeOfCollider.Boomerang;
    // Projectile
    [ShowIf("projectileOrBoomerang"), AllowNesting]
    public float Distance = 5f;
    [ShowIf("projectileOrBoomerang"), AllowNesting]
    public float Speed = 10f;
    [ShowIf("TypeOfPrefab", TypeOfCollider.Projectile), AllowNesting]
    public bool CrossEnemy = false;

    // Boomerang
    [ShowIf("TypeOfPrefab", TypeOfCollider.Boomerang), AllowNesting]
    public float TimeStopped = 0;
    [ShowIf("TypeOfPrefab", TypeOfCollider.Boomerang), AllowNesting]
    public float MinDistanceBack = 0.1f;

    [Header("Extra atributes"), Tooltip("Penetration 0 - 0.75")]
    [SerializedDictionary("Extra atribute", "Value")]
    public SerializedDictionary<ExtraDamageContextAtributes, float> ExtraAtributes;

    public DamageAtributes(DamageAtributes source) {
        DamageType = source.DamageType;
        UnitsToHit = source.UnitsToHit;
        Damage = source.Damage;
        HitShield = source.HitShield;
        BreakShield = source.BreakShield;
        Size = source.Size;
        TypeOfPrefab = source.TypeOfPrefab;
        HitBoxDuration = source.HitBoxDuration;
        DamageCooldown = source.DamageCooldown;
        Distance = source.Distance;
        Speed = source.Speed;
        CrossEnemy = source.CrossEnemy;
        TimeStopped = source.TimeStopped;
        MinDistanceBack = source.MinDistanceBack;

        ExtraAtributes = new SerializedDictionary<ExtraDamageContextAtributes, float>();
        foreach (var kvp in source.ExtraAtributes)
            ExtraAtributes.Add(kvp.Key, kvp.Value);
    }
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
