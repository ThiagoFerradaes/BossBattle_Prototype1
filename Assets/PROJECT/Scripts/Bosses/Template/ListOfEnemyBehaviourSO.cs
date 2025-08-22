using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ListOfEnemyAttacksSO")]
public class ListOfEnemyBehaviourSO : ScriptableObject
{
    public List<EnemyBehaviourSO> ListOfEnemyBehaviours;
}
