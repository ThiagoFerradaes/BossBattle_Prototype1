using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingField", menuName = "Interaction/TrainingField")]
public class TrainingFieldSO : InteractionSO
{
    public override Task Execute(DialogueSystem dialogue, PlayerInteractionManager playerInteractionManager)
    {
        playerInteractionManager.TrainingFieldOpen();
        return Task.CompletedTask;
    }
}
