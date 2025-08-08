using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives /WaponMasterPassive")]
public class WeaponMasterPassiveSO : PassiveSO {

    [Foldout("Axe State")] public float AmountOfFirstShieldRecieved;
    [Foldout("Axe State")] public float ShieldDuration;

    [Foldout("Gun State"), Range(0,1)] public float AttackSpeedBuff;
    [Foldout("Gun State")] public float AttackSpeedBuffDuration;
}
