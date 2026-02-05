using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ GreenAura")]
public class GraciaGreenAuraSO : CommonSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string attackAnimationParameter;
    [Foldout("Specific")] public string attackAnimationName;

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill typeOfSkill;
    [Foldout("Specific")] public GraciaAura typeOfAura;
    [Foldout("Specific")] public float amountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float skillDuration;
    [Foldout("Specific")] public float shieldDuration;
    [Foldout("Specific")] public List<float> shieldAmountPerLevel;
}
