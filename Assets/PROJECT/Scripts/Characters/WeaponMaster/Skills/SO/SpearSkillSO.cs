using UnityEngine;

[CreateAssetMenu(menuName = "Skills / SpearAttack")]
public class SpearSkillSO : SkillSO
{
    [Header("Animation")]
    public string SpearAttackTriggerName;
    public string AnimationName;

    [Header("Atributes")]
    public float Cooldown;
    public float Damage;
    public float HitBoxDuration;
    public float Penetration;
    public bool HitShield;
    public string SpearName;
    public Tags EnemyTag;
    public DamageType DamageType;
    public GameObject SpearPrefab;
    public Vector3 HitBoxPosition;
    public Vector3 WeaponPosition;
}
