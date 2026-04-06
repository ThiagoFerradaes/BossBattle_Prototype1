using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(menuName = "Dialogue/Children Dialogue")]
public class DialogueNode : ScriptableObject
{
    public LocalizedString DialogueText;
    public Character PrimaryCharacter;
    public ExpressionTypeDialogue PrimaryCharacterExpression;

    public bool hasSecondaryCharacterExpression;
    [ShowIf("hasSecondaryCharacterExpression")] public Character SecondaryCharacter;
    [ShowIf("hasSecondaryCharacterExpression")] public ExpressionTypeDialogue SecondaryCharacterExpression;

    public List<DialogueResponse> Responses;

    internal bool IsLastNode()
    {
        return Responses == null || Responses.Count == 0;
    }
}
