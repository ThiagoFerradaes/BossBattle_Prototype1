using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Tonfas")]
public class CyrusTonfasSkillSO : UltimateSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;
    [Foldout("Specific")] public string AnimationSpeedParameter;

    [Header("Atributes")]
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public float EnergyCostLevelOne;
    
    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float SizeLevelTwo;
    [Foldout("Specific")] public float AnimationSpeedLevelTwo;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float CritRateLevelThree;
    [Foldout("Specific")] public float CritDamageLevelThree;

}
