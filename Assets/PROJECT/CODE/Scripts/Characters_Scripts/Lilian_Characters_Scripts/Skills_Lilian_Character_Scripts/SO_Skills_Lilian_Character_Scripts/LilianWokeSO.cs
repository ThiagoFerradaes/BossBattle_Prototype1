using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ Woke")]
public class LilianWokeSO : CommonSkillSO {
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Buff")]
    [Foldout("Specific"), Range(0,1)] public float AncestralDamageBuffPercent;

}
