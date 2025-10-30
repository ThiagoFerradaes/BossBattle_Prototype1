using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Reward {
    public BossRewardItem Item;
    public float ChanceToObtainItem;
    public int Amount;
}

[CreateAssetMenu(menuName = "Bosses/ BossReward/ Generic")]
public class BossRewardSO : ScriptableObject
{
    public List<Reward> ListOfRewards = new();
    public List<Phases> PhasesToUnlock;
    public Character CharacterToUnlock;
    public virtual void WinRewards() {
        if (PhasesToUnlock.Count > 0) {
            foreach (var phase in PhasesToUnlock) {
                WhiteBoard.Instance.UnlockPhase(phase);
            }
        }

        if (CharacterToUnlock != Character.Null) {
            WhiteBoard.Instance.UnlockCharacter(CharacterToUnlock);
        }

        if (ListOfRewards.Count > 0) {
            foreach (var item in ListOfRewards) {
                int amount = 0;
                for (int i = 0; i < item.Amount; i++) {
                    float rng = Random.Range(0, 100);
                    if (rng < item.ChanceToObtainItem) {
                        amount++;
                    }
                }
                WhiteBoard.Instance.RecieveBossItem(item.Item, amount);
            }
        }
    }
}
