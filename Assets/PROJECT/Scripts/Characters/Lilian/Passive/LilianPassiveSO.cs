using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/LilianPassive")]
public class LilianPassiveSO : PassiveSO {

    [Header("Atributes")]
    [Foldout("Specific")]public float TimeToJudgment;
    [Foldout("Specific")] public float MaxAmountOfTributes;

    [Header("Blessing")]
    [Foldout("Specific")] public float BlessingCost;
    [Foldout("Specific")] public float BlessingDuration;
    [Foldout("Specific")] public float BlessingHealing;
    [Foldout("Specific")] public float BlessingHealingCooldown;
    [Foldout("Specific")] public float BlessingSize;
    [Foldout("Specific")] public string BlessingObjectName;
    [Foldout("Specific")] public GameObject BlessingObject;
    [Foldout("Specific")] public List<Tags> ListOfTags;

    [Header("Wrath")]
    [Foldout("Specific")] public float WrathStunDuration;
    [Foldout("Specific")] public float WrathDamage;
    [Foldout("Specific")] public int MaxAmountOfCorruption;

    [Header("UI")]
    [Foldout("Specific")] public GameObject LilianUI;
}
