using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/RootDialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueNode RootNode;
}
