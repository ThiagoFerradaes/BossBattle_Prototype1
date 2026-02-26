using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Shuriken")]
public class CyrusShurikenSkillSO : UltimateSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Level Zero Atributes")]
    [Foldout("Specific")] public float RotationSpeed;
    [Foldout("Specific")] public float InitialAngle;
    [Foldout("Specific")] public float Radius;
    [Foldout("Specific")] public int AmountOfShurikensLevelZero;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;

    [Header("Level One Atributes")]
    [Foldout("Specific")] public int AmountOfShurikensLevelOne;
    [Foldout("Specific")] public float PenetrationLevelOne;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public int AmountOfShurikensLevelTwo;
    [Foldout("Specific")] public float RotationSpeedLevelTwo;

    [Header("Level Two Atributes")]
    [Foldout("Specific")] public float RotationSpeedLevelThree;
}
