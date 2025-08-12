using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ListOfEnemyAttacksSO")]
public class ListOfEnemyAttacksSO : ScriptableObject
{
    public List<EnemySkillSO> ListOfEnemyAttacks;
}
