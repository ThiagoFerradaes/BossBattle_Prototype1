using UnityEngine;

public enum CrabArenaState { LowTide, IncomingTide,  HighTide, OutgoingTide }

[CreateAssetMenu(menuName = "Crab/ Arena")]
public class ArenaCrabSO : ScriptableObject
{
    public CrabArenaState InitialState = CrabArenaState.LowTide;

    [Header("Low Tide Atributes")]
    public float DurationOfLowTide = 15;

    [Header("Incoming Tide Atributes")]
    public float DurationOfIncomingTide = 5;

    [Header("Height Tide Atributes")]
    public float DurationOfHeightTide = 15;

    [Header("Outgoing Tide Atributes")]
    public float DurationOfOutgoingTide = 0;

}
