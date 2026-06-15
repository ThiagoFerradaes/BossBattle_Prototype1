using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Tonfas")]
public class CyrusTonfasSkillSO : UltimateSkillSO
{
    [Header("Atributes")]
    [Foldout("Specific")] public List<Sprite> ListOfSprites;
    [Foldout("Specific")] public List<AK.Wwise.Switch> ListOfSwitches;

    [Header("Max Buffs")]
    [Foldout("Specific")] public float SizeUpgrade;

}
