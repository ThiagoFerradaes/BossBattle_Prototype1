using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Dash")]
public class DashSO : CommonSkillSO
{
    [Header("Animation")]
    [Foldout("Specific")] public string AnimationParameter;
    [Foldout("Specific")] public string AnimationName;

    [Header("Atributes")]
    [Foldout("Specific")] public float DashDuration;
    [Foldout("Specific")] public float DashForce;
    [Foldout("Specific")] public float TimeToStartDash;
}
