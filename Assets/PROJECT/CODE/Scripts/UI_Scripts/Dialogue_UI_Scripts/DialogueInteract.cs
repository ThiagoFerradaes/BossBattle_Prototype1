using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine;

public class DialogueInteract : MonoBehaviour, IInteractable {


    [SerializeField] DialoguesByCharacter listOfDialogues;
    List<DialogueWithParams> _sortedList = new();

    public void Interact(PlayerInputHandlerManager handler) {
        handler.SetCanInput(false);

        Dialogue dialogue = ReturnADialogue();

        if (dialogue != null) DialogueManager.Instance.InitializeDialogue(dialogue.RootNode, handler);
    }

    Dialogue ReturnADialogue() {

        if (listOfDialogues.ListOfDialogueWithParams != null && listOfDialogues.ListOfDialogueWithParams.Count > 0) {

            _sortedList.Clear();
            _sortedList.AddRange(listOfDialogues.ListOfDialogueWithParams);
            _sortedList.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            for (int i = 0; i < _sortedList.Count; i++) {
                if (_sortedList[i].Parameter.CheckParams()) {
                    return _sortedList[i].Dialogue;
                }
            }

        }

        return listOfDialogues.DefaultDialogue;
    }
}
