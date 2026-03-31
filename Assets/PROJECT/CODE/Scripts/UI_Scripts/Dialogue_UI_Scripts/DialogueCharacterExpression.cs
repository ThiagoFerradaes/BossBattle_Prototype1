using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Character Expression List")]
public class DialogueCharacterExpression : ScriptableObject
{
    [SerializedDictionary("Expression", "Sprite")] public SerializedDictionary<ExpressionTypeDialogue, Sprite> dictionaryOfExpressions;
}
