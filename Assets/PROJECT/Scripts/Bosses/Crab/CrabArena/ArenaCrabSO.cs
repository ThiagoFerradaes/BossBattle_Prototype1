using System.Collections.Generic;
using UnityEngine;

public enum CrabArenaState { LowTide, IncomingTide,  HighTide, OutgoingTide }

[CreateAssetMenu(menuName = "Crab/ Arena")]
public class ArenaCrabSO : ScriptableObject
{
    [Header("Arena Atributes")]
    public CrabArenaState InitialState = CrabArenaState.LowTide;
    public List<GameObject> ListOfPlatforms = new();

    [Header("Low Tide Atributes")]
    public float DurationOfLowTide = 15;
    public float PlatformDownSpeed = 5;
    public float PlatformLowTideHeight = 0.5f;

    [Header("Incoming Tide Atributes")]
    public float DurationOfIncomingTide = 5;

    [Header("High Tide Atributes")]
    public float DurationOfHeightTide = 15;
    public float PlatformUpSpeed = 5;
    public float PlatformHighTideHeight = 4;

    [Header("Outgoing Tide Atributes")]
    public float DurationOfOutgoingTide = 0;

}
