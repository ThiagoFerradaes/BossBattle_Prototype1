using UnityEngine;



[CreateAssetMenu(menuName = "Characters/ Passives/ Cyrus")]
public class CyrusPassiveSO : PassiveSO {

    [Header("UI")]
    public GameObject CyrusUI;

    [Header("Rank")]
    public float DefenseAtRankSS;
    public float ExtraAttackAtRankSS;
    [Range(0, 1)] public float PercentOfRankPointsToIncrease;

    [Header("Sound")]
    public AK.Wwise.Event RankUpSound;
}
