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
    public Vector3 HitBoxPosition;
}
