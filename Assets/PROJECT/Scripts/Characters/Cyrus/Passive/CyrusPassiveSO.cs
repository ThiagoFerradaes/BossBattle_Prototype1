using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "Passives /WaponMasterPassive")]
public class CyrusPassiveSO : PassiveSO {
    
    [Foldout("Axe State")] public float AmountOfFirstShieldRecieved;
    [Foldout("Axe State")] public float ShieldDuration;

    [Foldout("Gun State"), Range(0,1)] public float AttackSpeedBuff;
    [Foldout("Gun State")] public float AttackSpeedBuffDuration;

    [Header("Dictionarys")]
    [SerializedDictionary("Classification", "AmountOfExp")]
    public SerializedDictionary<CyrusRank, float> AmountOfExpPerClassification;
    [SerializedDictionary("Classification", "Sprite")]
    public SerializedDictionary<CyrusRank, Sprite> SpritePerClassification;

    public InputAction UpgradeSkillOne, UpgradeSkillTwo, UpgradeUltimate;

    public GameObject CyrusUI;
}
