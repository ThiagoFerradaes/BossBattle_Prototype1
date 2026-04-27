using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Passives/ BastianPassive")]
public class BastianPassiveSO : PassiveSO
{
    [Header("Heat Areas")]
    public float MaxHeat;
    public float HeatToHitHeatArea;
    public float HeatToHitSuperHeatArea;
    public float HeatToHitOverHeatArea;
    public float HeatToHitLastOverHeatArea;
    public Color CoolColor, HeatColor, SuperHeatColor, OverHeatColor, LastOverHeatColor;
    public string CoolText, HeatText, SuperHeatText, OverHeatText, LastOverHeatText;

    [Header("Attack Speed Gain")]
    [Range(0,1)]public float AmountOfAttackSpeedGainHeat;
    [Range(0,1)]public float AmountOfAttackSpeedGainSuperHeat;

    [Header("Loose Heat")]
    public float HeatLostPerTime;
    public float TimeToLooseHeat;
    public float TimeToLooseAllHeatAfterLastHit;

    [Header("Canvas")]
    public GameObject HeatCanvas;

    [Header("Loose Health")]
    public float PercentOfMaxHealthLostPerTimeSuperHeat;
    public float PercentOfMaxHealthLostPerTimeOverHeat;
    public float PercentOfMaxHealthLostPerTimeExtremeHeat;
    public float TimeToLooseHealth;
    public HeatArea MinAreaToLooseHealth;
}
