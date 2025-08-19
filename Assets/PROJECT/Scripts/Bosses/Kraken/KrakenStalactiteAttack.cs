using UnityEngine;

[CreateAssetMenu(menuName = "Kraken / RainAttack")]
public class KrakenStalactiteAttack : EnemySkillSO {
    public float AttackDuration;
    public float CooldownBetweenEachStalactite;
    public float StalactiteFallSpeed;
    public float StalactiteDistanceFromEachOther;
    public float StalactiteMinDamage;
    public float StalactiteMaxDamage;
    public float StalactiteRange;
    public float StalactiteHeight;
}
