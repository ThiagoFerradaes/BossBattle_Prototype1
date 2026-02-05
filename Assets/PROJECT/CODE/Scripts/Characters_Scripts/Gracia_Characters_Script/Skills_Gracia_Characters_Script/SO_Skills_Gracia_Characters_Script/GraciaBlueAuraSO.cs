using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ BlueAura")]
public class GraciaBlueAuraSO : CommonSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string attackAnimationParameter;
    [Foldout("Specific")] public string attackAnimationName;

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill typeOfSkill;
    [Foldout("Specific")] public GraciaAura typeOfAura;
    [Foldout("Specific")] public float amountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float skillDuration;
    [Foldout("Specific")] public float cooldownToHit;
    [Foldout("Specific")] public List<DamageAtributes> attackAtributesList;
}
