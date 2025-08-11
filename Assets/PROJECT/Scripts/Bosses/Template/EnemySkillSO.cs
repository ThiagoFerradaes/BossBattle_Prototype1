using System.Collections.Generic;
using UnityEngine;


public class EnemySkillSO : ScriptableObject
{
    public int Priority;
    public float Cooldown;
    public List<SkillAnimationEvent> _listOfPrefabs;
}
