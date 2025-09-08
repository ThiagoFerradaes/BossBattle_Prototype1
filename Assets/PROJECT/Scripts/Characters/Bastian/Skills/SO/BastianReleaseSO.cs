using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/Release")]
public class BastianReleaseSO : CommonSkillSO
{
    public float HeatLost;
    [Range(0,1)]public float AttackSpeedGain;
    public float AttackSpeedDuration;

    public string AnimationParameter;
    public string AnimationName;
}
