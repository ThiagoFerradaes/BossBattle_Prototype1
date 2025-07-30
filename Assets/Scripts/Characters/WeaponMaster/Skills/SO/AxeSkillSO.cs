using UnityEngine;

[CreateAssetMenu(menuName = "Skills / AxeSkill")]
public class AxeSkillSO : SkillSO
{
    [Header("Animation")]
    public string FirstAnimationParameterName;
    public string SecondAnimationParameterName;
    public string SecondAnimationName;

    [Header("Atributes")]
    public float Cooldown;
    public float MinimalChargeTime;
    public float MaxChargeTime;
    public float MinDamage;
    public float MaxDamage;
    public float HitBoxDuration;
    public Tags EnemyTag;
    public Vector3 HitBoxPosition;
}
