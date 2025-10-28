using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueObjectSO", menuName = "Interaction/MapObjectSO")]
public class MapObjectSO : InteractionSO
{
    
    public override Task Execute(DialogueSystem dialogue, PlayerInteractionManager playerInteractionManager)
    {
        playerInteractionManager.OpenMap();
        return Task.CompletedTask;
    }
}
