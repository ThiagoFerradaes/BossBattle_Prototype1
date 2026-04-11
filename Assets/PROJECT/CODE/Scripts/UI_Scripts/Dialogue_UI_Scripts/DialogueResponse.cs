using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class DialogueResponse
{
    public LocalizedString ResponseText;
    public DialogueNode NextNode;
    public List<ResponseConsequence> ResponseConsequencesList = new();
}
