using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueObjectSO", menuName = "Interaction/DialogueObjectSO")]
public class DialogueObjectSO : InteractionSO
{
    [SerializeField] private DialogueSystemSo dialogue;
    
    public override async Task Execute(DialogueSystem dialogueSystem, PlayerInteractionManager playerInteractionManager)
    {
        dialogueSystem.gameObject.SetActive(true);
        dialogueSystem.OnComplicitEvent += playerInteractionManager.EndDialogue;
        await dialogueSystem.NewDialogue(dialogue);
    }
    
}
