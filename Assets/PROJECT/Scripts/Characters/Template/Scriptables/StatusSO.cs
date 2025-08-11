using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { MaxHealth, MaxAmountOfShield, BaseAttack, SkillAttack, Defense, SkillDefense
        , MoveSpeed, AttackSpeed, CritRate, CritDamage }

[Serializable]
public class Status {
    public StatusType Type;
    public float Value;
}

[CreateAssetMenu(menuName = "Status / StatusSO")]
public class StatusSO : ScriptableObject
{
    [Tooltip(
    "MaxAmountOfShield: value between 0 and 1 (represents % of max shield relative to health).\n" +
    "CritDamage: value greater than 1 (e.g., 2 means 200% critical damage).\n" +
    "CritRate: value between 0 and 1 (represents critical hit chance, e.g., 0.25 = 25%).\n" +
    "BaseAttack and SkillAttack: absolute base damage values for physical and magical attacks.\n" +
    "Defense and SkillDefense: used to reduce incoming damage, following the formula 100 / (100 + Defense).\n" +
    "MoveSpeed and AttackSpeed: affect movement and attack rates, do not directly influence damage.\n" +
    "Critical values should respect these ranges to ensure proper balance and correct calculations."
)]

    public List<Status> StatusList;
}


