using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Reward {
    public BossRewardItem Item;
    public float ChanceToObtainItem;
    public int Amount;
}
public abstract class BossRewardSO : ScriptableObject
{
    public List<Reward> ListOfRewards = new();
    public abstract void WinRewards();
}
