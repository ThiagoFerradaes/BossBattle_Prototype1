using System.Collections.Generic;
using UnityEngine;


//[System.Serializable]
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Children Dialogue")]
public class DialogueNode : ScriptableObject
{
    public string DialogueText;
    public List<DialogueResponse> Responses;

    internal bool IsLastNode()
    {
        return Responses == null || Responses.Count == 0;
    }
}
