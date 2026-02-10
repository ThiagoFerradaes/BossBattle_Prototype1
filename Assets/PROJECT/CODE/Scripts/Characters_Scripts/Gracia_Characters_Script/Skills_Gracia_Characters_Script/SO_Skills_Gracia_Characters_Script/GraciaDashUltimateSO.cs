using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Gracia/ DashUltimate")]
public class GraciaDashUltimateSO : UltimateSkillSO
{
    [Header("Animations")]
    [Foldout("Specific")] public string AttackAnimationParameter;
    [Foldout("Specific")] public string AttackAnimationName;

    [Header("Dash Atributes")]
    [Foldout("Specific")] public float DashForce;
    [Foldout("Specific")] public float DashDuration;
    [Foldout("Specific")] public float TimeToStartDash;
    [Foldout("Specific")] public List<float> DamageIncreasePerLevel;

    [Header("Blue Dash")]
    [Foldout("Specific")] public float BlueCooldownToMove;
    [Foldout("Specific")] public float BlueDistanceLimitToPlayer;
    [Foldout("Specific")] public float BlueShadowDurationToReachPlayer;
    [Foldout("Specific")] public GameObject BlueShadowPrefab;
    [Foldout("Specific")] public DamageAtributes BlueAtributes;
    [Foldout("Specific"), Range(0, 1)] public float PercentOfPassiveBarBlue;

    [Header("Yellow Dash")]
    [Foldout("Specific"), Range (0,1)] public float EnergyPercentToReturn;
    [Foldout("Specific"), Range(0, 1)] public float PercentOfPassiveBarYellow;

    [Header("Green Dash")]
    [Foldout("Specific"), Range(0, 1)] public float ShieldPercentToHeal;
    [Foldout("Specific"), Range(0, 1)] public float PercentOfPassiveBarGreen;

    [Header("Red Dash")]
    [Foldout("Specific"), Range(0, 100)] public float RedCritRate;
    [Foldout("Specific")] public float RedCritDamage;
    [Foldout("Specific"), Range(0, 1)] public float PercentOfPassiveBarRed;
}
