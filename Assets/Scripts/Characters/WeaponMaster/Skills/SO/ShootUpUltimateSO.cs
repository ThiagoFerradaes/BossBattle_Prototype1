using UnityEngine;

[CreateAssetMenu( menuName = "Skills / ShootUpUltimate")]
public class ShootUpUltimateSO : SkillSO
{
    [Header("Animation")]
    public string AnimationParameterTrigger;
    public string AnimationName;

    [Header("Atributes")]
    public float Cooldown;
    public float Damage;
    public float DamageCooldown;
    public float Duration;
    public Tags EnemyTag;
}
