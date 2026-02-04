using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ RedAura")]
public class GraciaRedAuraSO : CommonSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string AttackAnimationParameter;
    [Foldout("Specific")] public string AttackAnimationName;

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill TypeOfSkill;
    [Foldout("Specific")] public GraciaAura TypeOfAura;
    [Foldout("Specific")] public float AmountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float SkillDuration;
    [Foldout("Specific")] public List<float> CritDamageIncreaseList;
    [Foldout("Specific"), Tooltip("Values In Percent (0% - 100%)")] public List<CritRatePerAttackIndex> AditionalCriRateList;
}
[System.Serializable]
public struct CritRatePerAttackIndex {
    public float FirstAttackCritRateValue;
    public float SecondAttackCritRateValue;
    public float ThirdAttackCritRateValue;

    public CritRatePerAttackIndex(float critOne, float critTwo, float critThree) {
        FirstAttackCritRateValue = critOne;
        SecondAttackCritRateValue = critTwo;
        ThirdAttackCritRateValue = critThree;
    }
}
