using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ ListOfEnemyAttacksSO")]
public class ListOfEnemyBehaviourSO : ScriptableObject
{
    public EnemyBehaviourSO DefaultBehaviour;
    public List<EnemyBehaviourSO> ListOfEnemyBehaviours;
}
