using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/ CrabHighTideBomb")]
public class CrabHighTideBombDeathSO : DeathBehaviourGODesactiveSO
{
    public override void Death(GameObject parent)
    {
        CrabArenaManager.Instance.HighTideBomb();
        base.Death(parent);
    }
}
