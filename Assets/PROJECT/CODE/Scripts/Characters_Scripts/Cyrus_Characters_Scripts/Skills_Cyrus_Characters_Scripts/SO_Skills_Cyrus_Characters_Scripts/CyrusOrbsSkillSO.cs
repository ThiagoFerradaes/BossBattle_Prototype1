using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Orb")]
public class CyrusOrbsSkillSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Atributes")]
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public float OrbSpeedLevelOne;
    [Foldout("Specific")] public float OrbCooldownLevelOne;
    
    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float OrbCritRateLevelTwo;

    [Header("Level Three Atributes")]
    [Foldout("Specific")] public float MaxAmountOfOrbs;
    [Foldout("Specific")] public float TimeHoldingOrb;
    [Foldout("Specific")] public float TimeBetweenEachOrb;

}
