using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "Passives /WaponMasterPassive")]
public class CyrusPassiveSO : PassiveSO {

    [Header("UI")]
    public GameObject CyrusUI;

    [Header("SS Rank Buffs")]
    [SerializedDictionary("Status", "Buff Percent")] public SerializedDictionary<StatusType, float> ListOfStatusToBuff;
}
