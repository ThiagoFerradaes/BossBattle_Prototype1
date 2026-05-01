using System;
using UnityEngine;

public class TavernProgressionManager : MonoBehaviour
{
    private void Start() {
        if (ProgressWhiteBoard.Instance.DictionaryOfProgressBools[ProgressBools.IsKrakenDefeated]) {
            HandleKrakendDefeated();
        }
    }

    void HandleKrakendDefeated() {

        WhiteBoard.Instance.UnlockCharacter(Character.Cyrus);
    }

}
