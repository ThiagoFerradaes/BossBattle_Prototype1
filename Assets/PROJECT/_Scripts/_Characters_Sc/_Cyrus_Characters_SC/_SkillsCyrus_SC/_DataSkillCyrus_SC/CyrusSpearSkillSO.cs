using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ SpearAttack")]
public class CyrusSpearSkillSO : CommonSkillSO
{

    [Header("Max Buffs")]
    [Foldout("Specific")] public float UpgradeRange;
    [Foldout("Specific")] public float UpgradeCooldown;
    [Foldout("Specific")] public List<Sprite> ListOfSprites;
    [Foldout("Specific")] public List<AK.Wwise.Switch> ListOfSwitches;
}
