using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleRankSO")]
public class BattleRankSO : ScriptableObject
{
    [Header("Ranks")]
    public float MinTimeMultiplierValue;
    public float MaxTimeMultiplierValue;
    public float MaxTimeNoDamageTaken;
    public LayerMask EnemyLayer, PlayerLayer;
    [Tooltip("The total amount of hits to get to the next rank")] public SerializedDictionary<BattleRank, float> DictionaryOfRanksPoints;

    [Header("Combos")]
    public float ComboCooldown;
    public float ComboMaxDuration;
    [Tooltip("The total amount of hits to get to the next combo")] public SerializedDictionary<Combo, int> DictionaryOfHitsPerCombo;
    [Tooltip("The multiplier based on the current combo")] public SerializedDictionary<Combo, float> DictionaryOfMultipliersByCombo;
}
