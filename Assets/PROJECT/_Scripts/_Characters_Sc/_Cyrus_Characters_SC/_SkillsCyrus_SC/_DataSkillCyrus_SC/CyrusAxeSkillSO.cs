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
    [Foldout("Specific")] public List<AK.Wwise.Switch> ListOfSwitches;

    [Header("Max Buffs")]
    [Foldout("Specific")] public float AmountOfShield;
    [Foldout("Specific")] public float ShieldDuration;

}
