using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/LilianPassive")]
public class LilianPassiveSO : PassiveSO {

    public float TimeToJudgment;

    public float MaxAmountOfTributes;

    public float BlessingCost;
    public float BlessingDuration;
    public float BlessingHealing;
    public float BlessingHealingCooldown;
    public float BlessingSize;
    public string BlessingObjectName;
    public GameObject BlessingObject;
    public List<Tags> ListOfTags;

    public float WrathStunDuration;
    public float WrathDamage;
    public int MaxAmountOfCorruption;

    public GameObject LilianUI;
}
