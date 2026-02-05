using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ YellowAura")]
public class GraciaYellowAuraSO : CommonSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string attackAnimationParameter;
    [Foldout("Specific")] public string attackAnimationName;

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill typeOfSkill;
    [Foldout("Specific")] public GraciaAura typeOfAura;
    [Foldout("Specific")] public float amountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float skillDuration;
    [Foldout("Specific"), Tooltip("Values between 0 and 1")] public List<RangeFloatOne> attackSpeedBuffList;
}

[System.Serializable]
public struct RangeFloatOne {
    [Range(0, 1)] public float Value;
}
