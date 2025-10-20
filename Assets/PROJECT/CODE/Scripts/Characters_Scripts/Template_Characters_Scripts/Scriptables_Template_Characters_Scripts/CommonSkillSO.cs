using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;


public abstract class CommonSkillSO : SkillSO
{
    [Foldout("Common SKill")]public float FlatEnergyGainPerHit;
    [Foldout("Common SKill")] public float Cooldown;

    [Foldout("Common SKill")] public DamageAtributes SkillDamageAtributes;

    [Foldout("Common SKill")] public int Charges = 1;
    [Foldout("Common SKill")] public float ChargeCooldown;
}
