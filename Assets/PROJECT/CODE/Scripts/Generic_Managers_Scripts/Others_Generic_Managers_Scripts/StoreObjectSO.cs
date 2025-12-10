using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreObjectSO", menuName = "Interaction/StoreObjectSO")]
public class StoreObjectSO : InteractionSO
{
    public override Task Execute(DialogueSystem dialogueSystem, PlayerInteractionManager playerInteractionManager)
    {
        playerInteractionManager.StoreOpen();
        return Task.CompletedTask;
    }
    
}
