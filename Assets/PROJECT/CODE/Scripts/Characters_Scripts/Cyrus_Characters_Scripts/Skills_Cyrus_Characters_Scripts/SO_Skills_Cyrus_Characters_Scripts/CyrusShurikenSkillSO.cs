using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Shuriken")]
public class CyrusShurikenSkillSO : UltimateSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Level Zero Atributes")]
    [Foldout("Specific")] public float RotationSpeed;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public float PenetrationLevelOne;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float RotationSpeedLevelTwo;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float RotationSpeedLevelThree;
}
