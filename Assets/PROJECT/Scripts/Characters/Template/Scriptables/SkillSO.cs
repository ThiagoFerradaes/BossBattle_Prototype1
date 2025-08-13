using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfSkillAnimationPrefab { Hitbox, VFX}
[System.Serializable]
public class SkillAnimationEvent {
    public float TimeToSpawnPreFab;
    public string PreFabName;
    public TypeOfSkillAnimationPrefab PrefabType;
    public GameObject PreFab;
    public Vector3 PreFabPosition;
}

public enum Tags { Enemy, Player }
public abstract class SkillSO : ScriptableObject
{
    [Header("Skill Manager")]
    [Foldout("Generic")]public SkillObjectManager SkillManagerObject;
    [Foldout("Generic")] public string SkillManagerName;

    [Header("Skill Prefabs")]
    [Foldout("Generic"), SerializedDictionary("Combo", "Event")]
    public SerializedDictionary<int, List<SkillAnimationEvent>> Prefabs;

    [Header("Skill Range Object")]
    [Foldout("Generic")] public GameObject SkillObjectRangeObject;
    [Foldout("Generic")] public string SkillObjectRangeName;

    [Header("Casting Skill options")]
    [Foldout("Generic")] public bool BlockWalkWhilePreCasting;
    [Foldout("Generic")] public bool BlockDashWhilePreCasting;
    [Foldout("Generic")] public bool PreCastOn = true;

    [Header("Skill Parameters")]
    [Foldout("Generic")] public Character SkillCharacter;
    [Foldout("Generic")] public bool Cancelable;

}
