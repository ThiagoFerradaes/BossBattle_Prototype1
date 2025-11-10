using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Shield")]
public class CyrusShieldSkillSO : CommonSkillSO {

    [Header("Animation")]
    [Foldout("Specific")] public string SpearAttackTriggerName;
    [Foldout("Specific")] public string AnimationName;

    [Header("Atributes")]
    [Foldout("Specific")] public float ShieldAmount;
    [Foldout("Specific")] public float ShieldDuration;

    [Header("Level Up Atributes")]
    [Foldout("Specific")] public float ShieldAmountLevelTwo;
    [Foldout("Specific")] public float ShieldDurationLevelTwo;
    [Foldout("Specific")] public float ShieldExplosionSizeLevelTwo;
    [Foldout("Specific"), Range(0,100)] public float IncreaseInAbyssalDamage;
    [Foldout("Specific"), Range(0, 100)] public float IncreaseInAncestralDamage;
    [Foldout("Specific"), Range(0, 100)] public float IncreaseInAttackSpeed;
    [Foldout("Specific"), SerializedDictionary("Level", "Amount")] public SerializedDictionary<int, int> AmountOfUsesPerLevel;
}
