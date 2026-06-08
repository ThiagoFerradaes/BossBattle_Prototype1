using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using UnityEngine;


public class DamageContext {
    public DamageAtributes Atributes;
    public StatusManager StatusManager;

    public DamageContext(DamageAtributes atributes, StatusManager status) {
        this.Atributes = atributes;
        this.StatusManager = status;
    }
}
[Serializable]
public class DamageAtributes {
    [Header ("Main Atributes")]
    public LayerMask UnitsToHit;

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
    //[ShowIf("TypeOfPrefab", TypeOfCollider.Projectile), AllowNesting]
    //public bool ExplodeInTheEnd = false;
    //[ShowIf("ExplodeInTheEnd"), AllowNesting, Tooltip("Leave TypeOfProjectile in Instant")]
    //public DamageAtributes ExplosionAtribute;
    //[ShowIf("ExplodeInTheEnd"), AllowNesting]
    //public GameObject ExplosionHitBox;

    // Boomerang
    [ShowIf("TypeOfPrefab", TypeOfCollider.Boomerang), AllowNesting]
    public float TimeStopped = 0;
    [ShowIf("TypeOfPrefab", TypeOfCollider.Boomerang), AllowNesting]
    public float MinDistanceBack = 0.1f;

    [Header("Extra atributes"), Tooltip("Penetration 0 - 75, Crit Rate 0 - 100, Crit Damage 100 - Infinity")]
    [SerializedDictionary("Extra atribute", "Value")]
    public SerializedDictionary<ExtraDamageContextAtributes, float> ExtraAtributes;

    public DamageAtributes(DamageAtributes source) {
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
    public static float CalculateDamage( // Considerando o crítico do personagem
        DamageAtributes atributes,
        StatusManager statusDealer,
        StatusManager statusReciever
        ) {

        float rawDamage = atributes.Damage * statusDealer.ReturnStatusValue(StatusType.BaseAttack);

        float targetDefense = statusReciever.ReturnStatusValue(StatusType.Defense); 

        float penetration = 0;
        
        if (atributes.ExtraAtributes.ContainsKey(ExtraDamageContextAtributes.Penetration))
            penetration = Mathf.Min(0.75f, atributes.ExtraAtributes[ExtraDamageContextAtributes.Penetration]/100);

        targetDefense *= (1 - penetration);

        float finalDamage = rawDamage * (100 / (100 + targetDefense));

        return (Mathf.Max(1, finalDamage));
    }

}
