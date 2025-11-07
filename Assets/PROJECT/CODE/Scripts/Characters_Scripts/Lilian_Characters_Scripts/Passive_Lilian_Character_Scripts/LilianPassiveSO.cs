using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Passives/ LilianPassive")]
public class LilianPassiveSO : PassiveSO {

    [Header("Atributes")]
    [Foldout("Specific")] public float MaxAmountOfTributes;
    [Foldout("Specific")] public float BlessingHealing;

    [Header("UI")]
    [Foldout("Specific")] public GameObject LilianUI;
}
