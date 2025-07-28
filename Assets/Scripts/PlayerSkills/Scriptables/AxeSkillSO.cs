using UnityEngine;

[CreateAssetMenu(menuName = "Skills / AxeSkill")]
public class AxeSkillSO : SkillSO
{
    [Header("Animação")]
    public string FirstAnimationParameterName;
    public string SecondAnimationParameterName;
    public string SecondAnimationName;

    [Header("Atributes")]
    public float Cooldown;
    public float MinimalChargeTime;
    public float MaxChargeTime;
    public float BaseDamage;
    public float MaxDamage;
    public Vector3 HitBoxPosition;
}
