using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/ Skills/ Bastian/ Dash")]
public class BastianDashSO : DashSO {

    [Header("Atributes")]
    [Foldout("Specific")] public float AmountOfHeatLost;
    [Foldout("Specific")] public AK.Wwise.Event DashSFX = null;
}
