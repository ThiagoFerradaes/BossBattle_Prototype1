using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ YellowAura")]
public class GraciaYellowAuraSO : CommonSkillSO, IGraciaSkill
{

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill TypeOfSkill;
    [Foldout("Specific")] public GraciaAura TypeOfAura;
    [Foldout("Specific")] public float AmountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float SkillDuration;
    [Foldout("Specific"), Tooltip("Values between 0 and 1")] public List<RangeFloatOne> AttackSpeedBuffList;

    public GraciaAura ReturnSkillAura()
    {
        return TypeOfAura;
    }
}

[System.Serializable]
public struct RangeFloatOne {
    [Range(0, 1)] public float Value;
}
