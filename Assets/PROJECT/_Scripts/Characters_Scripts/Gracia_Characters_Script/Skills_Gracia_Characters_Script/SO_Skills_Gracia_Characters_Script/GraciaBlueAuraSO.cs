using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ BlueAura")]
public class GraciaBlueAuraSO : CommonSkillSO, IGraciaSkill
{

    [Header("Skil lAtributes")]
    [Foldout("Specific")] public GraciaTypeOfSkill TypeOfSkill;
    [Foldout("Specific")] public GraciaAura TypeOfAura;
    [Foldout("Specific")] public float AmountOfValueGainedWhenUsed;
    [Foldout("Specific")] public float SkillDuration;
    [Foldout("Specific")] public float CooldownToHit;
    [Foldout("Specific")] public List<DamageAtributes> AttackAtributesList;

    #region Interface

    public GraciaAura ReturnSkillAura()
    {
        return TypeOfAura;
    }

    #endregion
}
