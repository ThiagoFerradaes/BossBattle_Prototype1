using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/ ListOfEnemyAttacksSO")]
public class ListOfEnemyBehaviourSO : ScriptableObject
{
    public List<EnemyBehaviourSO> ListOfEnemyBehaviours;
}
