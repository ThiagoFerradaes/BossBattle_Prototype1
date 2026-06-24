using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Lilian/ Woke")]
public class LilianWokeSO : CommonSkillSO {

    [Header("Buff")]
    [Foldout("Specific"), Range(0,1)] public float AncestralDamageBuffPercent;

}
