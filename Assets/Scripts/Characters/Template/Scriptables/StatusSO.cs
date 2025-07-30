using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { MaxHealth, MaxAmountOfShield, Attack, MoveSpeed, AttackSpeed, CritRate, CritDamage }

[Serializable]
public class Status {
    public StatusType Type;
    public float Value;
}

[CreateAssetMenu(menuName = "Status / StatusSO")]
public class StatusSO : ScriptableObject
{
    [Tooltip("If MaxAmountOfShield -> between 0 and 1, if CritDamage -> bigger than 100" +
        " if CritRate -> between 0 and 100")]
    public List<Status> StatusList;
}


