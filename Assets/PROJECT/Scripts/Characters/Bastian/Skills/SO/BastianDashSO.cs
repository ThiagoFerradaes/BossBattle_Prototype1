using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Bastian/Dash")]
public class BastianDashSO : DashSO {

    [Header("Atributes")]
    [Foldout("Specific")] public float AmountOfHeatLost;
}
