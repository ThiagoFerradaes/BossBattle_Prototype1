using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class DialogueResponse {
    public LocalizedString ResponseText;
    public DialogueNode NextNode;

    [Space(10)]
    [Header("Consequences")]
    public List<ResponseConsequence> ResponseConsequencesList = new();

    [Space(10)]
    [Header("Dialogue Nodes with Param")]
    [Tooltip("Se só tiver uma opção para a próxima resposta, deixa vazio")]
    public List<DialogueNodeWithParam> NodesWithParams;
}

[Serializable]
public class DialogueNodeWithParam {

    public List<DialogueParams> ListOfParameters;
    public DialogueNode Node;
    public int Priority;
}
