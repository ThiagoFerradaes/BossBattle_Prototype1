using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Characters/ Passives/ BastianPassive")]
public class BastianPassiveSO : PassiveSO {
    [Header("Heat Areas")]
    public float MaxHeat;
    public float HeatToHitHeatArea;
    public float AmountOfHeatToHitOverHeatArea;
    public LocalizedString CoolText, HeatText, OverHeatText;

    [Header("Audio")]
    public AK.Wwise.Event HeatZoneChangeSound;
    public List<AK.Wwise.Switch> HeatZoneSwitchs;
    public AK.Wwise.Event LooseHealthSound;

    [Header("Attack Speed Gain")]
    [Range(0, 1)] public float AmountOfAttackGainHeat;
    [Range(0, 1)] public float AmountOfAttackGainOverHeat;

    [Header("Loose Heat")]
    public float HeatLostPerTime;
    public float TimeToLooseHeat;
    public float TimeToLooseAllHeatAfterLastHit;

    [Header("Canvas")]
    public GameObject HeatCanvas;

    [Header("Loose Health")]
    [Tooltip("Values between 0 and 1")] public SerializedDictionary<BastianHeatArea, float> AmountOfHealthToLoosePerArea;
    public float HealthLostByHeatCooldown;
}
