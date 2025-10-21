using NaughtyAttributes;
using UnityEngine;


public abstract class CommonSkillSO : SkillSO {

    [Header("Skill Atributes")]
    [Foldout("Common SKill Atributes")] public float FlatEnergyGainPerHit;
    [Foldout("Common SKill Atributes")] public float Cooldown;

    [Header("Charges")]
    [Foldout("Common SKill Atributes")] public bool HasCharges = false;
    [Foldout("Common SKill Atributes"), ShowIf("HasCharges")] public int Charges = 1;
    [Foldout("Common SKill Atributes"), ShowIf("HasCharges")] public float ChargeCooldown;

    [Header("Damage Atributes")]
    [Foldout("Common SKill Atributes")] public DamageAtributes SkillDamageAtributes;
}
