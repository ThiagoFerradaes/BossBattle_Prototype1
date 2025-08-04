using UnityEngine;

[CreateAssetMenu( menuName = "Skills / ShootUpUltimate")]
public class ShootUpUltimateSO : SkillSO
{
    [Header("Animation")]
    public string AnimationParameterTrigger;
    public string AnimationName;
    public string LastAnimationName;

    [Header("Atributes")]
    public float Cooldown;
    public float Damage;
    public float DamageCooldown;
    public float Duration;
    public GameObject WeaponPrefab;
    public string WeaponName;
    public Tags EnemyTag;
    public Vector3 WeaponPosition;
}
