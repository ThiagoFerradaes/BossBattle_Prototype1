using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class DialogueResponse
{
    public LocalizedString ResponseText;
    public DialogueNode NextNode;
}
