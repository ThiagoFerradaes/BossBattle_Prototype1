using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public enum CrabArenaState { LowTide, IncomingTide,  HighTide, OutgoingTide }

[CreateAssetMenu(menuName = "Crab/ Arena")]
public class CrabArenaSO : ScriptableObject
{
    [Header("Arena Atributes")]
    public CrabArenaState InitialState = CrabArenaState.LowTide;
    public List<GameObject> ListOfPlatforms = new();

    [Header("Low Tide Atributes")]
    public float DurationOfLowTide = 15;
    public float PlatformDownSpeed = 5;
    public float PlatformLowTideHeight = 0.5f;

    [Header("Incoming Tide Atributes")]
    public float IncomingTideDuration = 5;
    public float IncomingTideAttackDamage;
    public float IncomingTideAttackDamageCooldown;
    public float IncomingTideAttackDamagSize;
    public string IncomingTideAttackHitBoxName;
    public GameObject IncomingTideAttackHitBox;

    [Header("High Tide Atributes")]
    public float DurationOfHeightTide = 15;
    public float PlatformUpSpeed = 5;
    public float PlatformHighTideHeight = 4;
    public float BombAmountOfFlatTimeReduced = 5;
    public float BombsCooldownToAppear = 5;
    public float BombHeight = 1;
    public float BombSize = 3;
    public GameObject BombPrefab;

    [Header("Outgoing Tide Atributes")]
    public float DurationOfOutgoingTide = 2;
    public float AnimalDistance = 5;
    public float AnimalHeight = 0.5f;
    public int AmountOfAnimals = 3;
    [SerializedDictionary("Name", "Prefab")]public SerializedDictionary<string, GameObject> ListOfAnimals = new();

}
