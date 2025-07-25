using System.Collections.Generic;
using UnityEngine;

public abstract class SkillSO : ScriptableObject
{
    [Header("Skill Manager")]
    public SkillObjectManager SkillManagerObject;
    public string SkillManagerName;

    [Header("Skill Prefabs")]
    public List<GameObject> SkillPrefabObjects = new();
    public List<string> SkillPrefabNames = new();

    [Header("Skill Range Object")]
    public GameObject SkillObjectRangeObject;
    public string SkillObjectRangeName;

    [Header("Casting SKill options")]
    public bool BlockWalkWhilePreCasting;
    public bool BlockDashWhilePreCasting;

    public abstract void UseSKill(Animator anim);
}
