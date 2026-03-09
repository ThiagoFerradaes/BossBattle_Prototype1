using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ SpearAttack")]
public class CyrusSpearSkillSO : CommonSkillSO
{

    [Header("Level Up Buffs")]
    [Foldout("Specific")] public float Level2Range;
    [Foldout("Specific")] public float Level3Penetration;
    [Foldout("Specific")] public float Level3Cooldown;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;
}
