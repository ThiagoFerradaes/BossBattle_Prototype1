using UnityEngine;

[CreateAssetMenu(menuName = "Skills / Dash")]
public class DashSO : SkillSO
{
    [Header("Animation")]
    public string AnimationParameter;
    public string AnimationName;

    [Header("Atributes")]
    public float DashDuration;
    public float DashForce;
    public float Cooldown;
    public float TimeToStartDash;
}
