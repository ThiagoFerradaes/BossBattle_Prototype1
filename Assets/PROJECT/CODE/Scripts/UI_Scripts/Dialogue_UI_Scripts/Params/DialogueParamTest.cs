using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Params/Test")]
public class DialogueParamTest : DialogueParams {

    [SerializeField] ProgressBools boolToCheck;
    [SerializeField] bool invert;

    public override bool CheckParams() {
        bool result = ProgressWhiteBoard.Instance.DictionaryOfProgressBools[boolToCheck];

        bool finalResult = invert ? !result : result;
        return finalResult;
    }
}
