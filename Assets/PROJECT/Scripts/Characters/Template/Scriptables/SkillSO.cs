using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillAnimationEvent {
    public float timeToSpawnHitBox;
    public GameObject hitboxPrefab;
    public string hitboxName;
}

public enum Tags { Enemy, Player }
public abstract class SkillSO : ScriptableObject
{
    [Header("Skill Manager")]
    public SkillObjectManager SkillManagerObject;
    public string SkillManagerName;

    [Header("Skill Prefabs")]
    public List<SkillAnimationEvent> Prefabs;

    [Header("Skill Range Object")]
    public GameObject SkillObjectRangeObject;
    public string SkillObjectRangeName;

    [Header("Casting SKill options")]
    public bool BlockWalkWhilePreCasting;
    public bool BlockDashWhilePreCasting;
    public bool PreCastOn = true;

    [Header("Skill Parameters")]
    public Character SkillCharacter;

}
