using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Release")]
public class BastianReleaseSO : CommonSkillSO {

    [Header("Atributes")]
    [Foldout("Specific")] public float HeatLost;
    [Foldout("Specific"), Range(0, 2)] public float AttackSpeedGain;
    [Foldout("Specific")] public float AttackSpeedDuration;
}
