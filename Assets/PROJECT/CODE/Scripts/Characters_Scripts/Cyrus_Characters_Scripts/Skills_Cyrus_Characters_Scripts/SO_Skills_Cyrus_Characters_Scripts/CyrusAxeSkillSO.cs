using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ AxeSkill")]
public class CyrusAxeSkillSO : CommonSkillSO
{
    [Header("Atributes")]
    [Header("Floats")]
    [Foldout("Specific")] public float MinimalChargeTime;
    [Foldout("Specific")] public float MaxChargeTime;
    [Foldout("Specific")] public float MinDamage;
    [Foldout("Specific")] public float MaxDamage;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;

    [Header("Level 1 & 2 Buffs")]
    [Foldout("Specific")] public float Level1AmountOfShield;
    [Foldout("Specific")] public float Level2AmountOfShield;
    [Foldout("Specific")] public float ShieldDuration;
    [Foldout("Specific")] public float NewMaxChargeTime;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Level 3 Buff")]
    [Foldout("Specific")] public DamageAtributes RocksAtributes;

}
