using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
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
    [SerializedDictionary("Boss", " Phase to Unlock"), SerializeField]
    SerializedDictionary<Bosses, int> DictionaryOfPhasesToUnlock = new();
    public Character CharacterToUnlock;
    public virtual void WinRewards() {
        if (DictionaryOfPhasesToUnlock.Count > 0) {
            foreach (var phase in DictionaryOfPhasesToUnlock.Keys) {
                var value = DictionaryOfPhasesToUnlock[phase];
                WhiteBoard.Instance.UnlockPhase(phase, value);
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
