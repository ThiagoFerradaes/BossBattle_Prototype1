using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Params/Test")]
public class DialogueParamTest : DialogueParams {
    [SerializeField] bool canPass;
    public override bool CheckParams() {
        return canPass;
    }
}
