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
    public bool IsTrueDamage;
    public Tags EnemyTag;
    public Vector3 HitBoxPosition;
}
