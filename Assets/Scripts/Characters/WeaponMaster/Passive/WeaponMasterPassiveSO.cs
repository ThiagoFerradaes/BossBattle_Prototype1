using UnityEngine;

[CreateAssetMenu(menuName = "Passives /WaponMasterPassive")]
public class WeaponMasterPassiveSO : PassiveSO
{
    [Header("Axe State")]
    public float amountOfFirstShieldRecieved;
    public float shieldDuration;
}
