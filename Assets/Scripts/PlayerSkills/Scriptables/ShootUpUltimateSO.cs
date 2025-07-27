using UnityEngine;

[CreateAssetMenu( menuName = "Skills / ShootUpUltimate")]
public class ShootUpUltimateSO : SkillSO
{
    public float Cooldown;
    public float Damage;
    public float DamageCooldown;
    public float Duration;
    public string EnemyTag;
}
