using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Response Consequences/Change Bool Response Consequence")]
public class ChangeBoolResponseConsequence : ResponseConsequence
{
    [SerializeField] List<ProgressBools> boolsToChange;
    [SerializeField] bool newValue;
    [SerializeField] bool isPreconsequence;

    public override void ExecutePreConsequece() {
        if (!isPreconsequence) return;

        foreach (ProgressBools boolToChange in boolsToChange) {
            ProgressWhiteBoard.Instance.DictionaryOfProgressBools[boolToChange] = newValue;
        }
    }
    public override void ExecuteConsequence() {
        if (isPreconsequence) return;

        foreach (ProgressBools boolToChange in boolsToChange) {
            ProgressWhiteBoard.Instance.DictionaryOfProgressBools[boolToChange] = newValue;
        }
    }
}
