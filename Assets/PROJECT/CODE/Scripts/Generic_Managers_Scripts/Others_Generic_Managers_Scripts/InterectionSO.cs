using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "InterectionSO", menuName = "Interaction/InteractionBaseSO")]
public abstract class InteractionSO : ScriptableObject
{
    public abstract Task Execute(DialogueSystem dialogue, PlayerInteractionManager playerInteractionManager);

}
