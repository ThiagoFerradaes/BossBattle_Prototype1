using UnityEngine;

[CreateAssetMenu(menuName = "Passives/BastianPassiveSO")]
public class BastianPassiveSO : PassiveSO
{
    public float MaxHeat;
    public float HeatToHitHeatArea;
    public float HeatToHitSuperHeatArea;
    public float HeatToHitOverHeatArea;
    public float HeatToHitLastOverHeatArea;

    [Range(0,1)]public float AmountOfAttackSpeedGain;

    public float HeatLostPerTime;
    public float TimeToLooseHeat;

    public GameObject HeatCanvas;

    public Color CoolColor, HeatColor, SuperHeatColor, OverHeatColor, LastOverHeatColor;
}
