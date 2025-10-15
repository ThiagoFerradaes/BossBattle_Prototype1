using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommonSkillSO : SkillSO
{
    [Foldout("Common SKill")]public float FlatEnergyGainPerHit;
    [Foldout("Common SKill")] public float Cooldown;

    [Foldout("Common SKill")] public List<Tags> EnemyTag;
    [Foldout("Common SKill")] public DamageType DamageType;
    [Foldout("Common SKill")] public bool HitShield;

    [Foldout("Common SKill")] public int Charges = 1;
    [Foldout("Common SKill")] public float ChargeCooldown;
}
