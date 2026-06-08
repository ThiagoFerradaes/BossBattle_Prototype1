using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Status {
    public StatusType Type;
    public float Value;

    public Status(StatusType type, float value) {
        Type = type;
        Value = value;
    }
}

[CreateAssetMenu(menuName = "Status / StatusSO")]
public class StatusSO : ScriptableObject {
    [Tooltip(
    "MaxAmountOfShield: value between 0 and 100 (represents % of max shield relative to health).\n" +
    "CritDamage: value greater than 1 (e.g., 2 means 200% critical damage).\n" +
    "CritRate: value between 0 and 100 (represents critical hit chance, e.g., 0.25 = 25%).\n" +
    "BaseAttack and SkillAttack: should be 1.\n" +
    "Defense and SkillDefense: used to reduce incoming damage, following the formula 100 / (100 + Defense).\n" +
    "MoveSpeed and AttackSpeed: affect movement and attack rates, do not directly influence damage.\n" +
    "Critical values should respect these ranges to ensure proper balance and correct calculations. \n" +
    "Energy Recharge should be 1"
)]

    public List<Status> StatusList = new() {
        {new(StatusType.MaxHealth, 100) },
        {new(StatusType.MaxAmountOfShield, 25) },
        {new(StatusType.BaseAttack, 1) },
        {new(StatusType.Defense, 100) },
        {new(StatusType.MoveSpeed, 5.5f) },
        {new(StatusType.AttackSpeed, 1) },
        {new(StatusType.EnergyRecharge, 1) },
    };
}


