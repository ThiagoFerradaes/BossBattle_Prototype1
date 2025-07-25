using UnityEngine;

[CreateAssetMenu(menuName = "Skills / SpearAttack")]
public class SpearSkillSO : SkillSO
{
    public string SpearAttackTriggerName;
    public float SpearAttackCooldown;
    public string AnimationName;
    public float Damage;
    public float HitBoxDuration;
    public Vector3 HitBoxPosition;
}
