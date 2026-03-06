using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ SteamPunch")]
public class BastianSteamPunchSO : CommonSkillSO {

    [Header("Animation Parameter")]
    [Foldout("Specific")] public string AttackSpeedAnimationParameter;

    [Header("Passive")]
    [Foldout("Specific")] public float HeatLoss;
    [Foldout("Specific")] public float PenetrationOnSuperHeat;
    [Foldout("Specific")] public float CritChanceOverHeat;
    [Foldout("Specific")] public float LastOverHeatCritDamage;
}
