using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Lilian/Sacrifice")]
public class LilianSacrificeSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Values")]
    [Foldout("Specific"),Range(0, 100)] public float PercentOfCurrentHealthToLoose;
    [Foldout("Specific")] public float AmountOfTributesGainPerHealthLost;
}
