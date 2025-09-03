using UnityEngine;

[CreateAssetMenu(fileName = "BastianPassiveSO", menuName = "Scriptable Objects/BastianPassiveSO")]
public class BastianPassiveSO : PassiveSO
{
    public float MaxHeat;
    public float HeatToHitHeatArea;
    public float HeatToHitSuperHeatArea;
    public float HeatToHitOverHeatArea;
    public float HeatToHitLastOverHeatArea;

    public float AmountOfAttackSpeedGain;

    public float HeatLostPerTime;
    public float TimeToLooseHeat;
}
