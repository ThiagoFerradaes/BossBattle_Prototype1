using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "Passives /WaponMasterPassive")]
public class CyrusPassiveSO : PassiveSO {
    
    [Header("Dictionarys")]
    [SerializedDictionary("Classification", "AmountOfExp")]
    public SerializedDictionary<CyrusRank, float> AmountOfExpPerClassification;
    [SerializedDictionary("Classification", "Sprite")]
    public SerializedDictionary<CyrusRank, Sprite> SpritePerClassification;

    [Header("Skill Upgrade Actions")]
    public InputAction UpgradeSkillOne, UpgradeSkillTwo, UpgradeUltimate;

    [Header("UI")]
    public GameObject CyrusUI;

    [Header("Passive gain of exp")]
    public float ExpGain;
    public float ExpGainCooldown;

    [Header("SkillCost")]
    public float PercentOfDefensesLost;
    public float PercentOfDefensesLostDuration;
}
