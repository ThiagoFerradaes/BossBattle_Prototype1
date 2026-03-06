using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Dards")]
public class CyrusDardsSkillSO : CommonSkillSO
{

    [Header("Level Zero Atributes")]
    [Foldout("Specific")] public float AmountOfDards;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public float AmountOfDefenseDrop;
    [Foldout("Specific")] public float DefenseDropDuration;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float CooldownLevelTwo;

    [Header("Level Three Atributes")]
    [Foldout("Specific")] public float AmountOfDardsLevelThree;
    [Foldout("Specific")] public float TimeBetweenDards;
}
