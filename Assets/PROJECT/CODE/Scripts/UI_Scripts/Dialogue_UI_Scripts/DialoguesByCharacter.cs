using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueWithParams {
    public int Priority;
    public DialogueParams Parameter;
    public Dialogue Dialogue;
}

[CreateAssetMenu(menuName = "Dialogue/DialogueByCharacter")]
public class DialoguesByCharacter : ScriptableObject {
    public Dialogue DefaultDialogue;
    public List<DialogueWithParams> ListOfDialogueWithParams = new();
}
