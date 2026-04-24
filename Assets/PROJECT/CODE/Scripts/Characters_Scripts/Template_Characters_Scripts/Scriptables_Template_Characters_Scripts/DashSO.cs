using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Cyrus/ Dash")]
public class DashSO : CommonSkillSO
{

    [Header("Atributes")]
    [Foldout("Specific")] public float DashDuration;
    [Foldout("Specific")] public float DashForce;
    [Foldout("Specific")] public float PercentOfAnimationToStartDash;
    [Foldout("Specific")] public AK.Wwise.Event DashSound;
}
