using UnityEngine;

public class DeathBehaviourGODesactiveSO : DeathBehaviourSO
{
    public override void Death(GameObject parent)
    {
        parent.SetActive(false);
    }
}
