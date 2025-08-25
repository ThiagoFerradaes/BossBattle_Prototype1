using UnityEngine;

public class KrakenReward : BossRewardSO {
    [SerializeField] Phases phaseToUnlock;
    [SerializeField] public Character characterToUnlock;
    public override void WinRewards() {
        if (phaseToUnlock != Phases.Null) {
            WhiteBoard.Instance.UnlockPhase(phaseToUnlock);
        }

        if (characterToUnlock != Character.Null) {
            WhiteBoard.Instance.UnlockCharacter(characterToUnlock);
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
