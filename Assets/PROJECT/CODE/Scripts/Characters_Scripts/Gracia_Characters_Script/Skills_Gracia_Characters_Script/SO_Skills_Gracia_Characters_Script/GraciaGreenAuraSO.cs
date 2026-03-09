using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ GreenAura")]
public class GraciaGreenAuraSO : CommonSkillSO, IGraciaSkill
{

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill typeOfSkill;
    [Foldout("Specific")] public GraciaAura TypeOfAura;
    [Foldout("Specific")] public float amountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float skillDuration;
    [Foldout("Specific")] public float shieldDuration;
    [Foldout("Specific")] public List<float> shieldAmountPerLevel;

    #region Interface

    public GraciaAura ReturnSkillAura()
    {
        return TypeOfAura;
    }

    #endregion
}
