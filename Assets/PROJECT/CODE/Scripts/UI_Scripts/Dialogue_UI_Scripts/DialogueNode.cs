using System.Collections.Generic;
using UnityEngine;


//[System.Serializable]
[CreateAssetMenu(menuName = "Dialogue/Children Dialogue")]
public class DialogueNode : ScriptableObject
{
    public string DialogueText;
    public TypeOfDialogueSprite SpriteType;
    public Character Character;
    public TypeOfExpression Expression;
    public List<DialogueResponse> Responses;

    internal bool IsLastNode()
    {
        return Responses == null || Responses.Count == 0;
    }
}
