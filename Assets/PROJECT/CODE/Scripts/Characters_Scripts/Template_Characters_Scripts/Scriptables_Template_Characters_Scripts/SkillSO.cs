using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfSkillPrefab { Hitbox, VFX, PreCastRange, Manager }
[System.Serializable]
public class SkillAnimationEvent {
    public float TimeToSpawnPreFab;
    public TypeOfSkillPrefab PrefabType;
    [ShowIf("PrefabType", TypeOfSkillPrefab.VFX), AllowNesting]
    public float PrefabDuration;
    public GameObject PreFab;
    public Vector3 PreFabPosition;
}

public enum Tags { Enemy, Player, Construct }
public class SkillSO : ScriptableObject {
    [Header("Skill Manager")]
    [Foldout("Generic")] public SkillObjectManager SkillManagerObject;

    [Header("Skill Prefabs")]
    [Foldout("Generic"), SerializedDictionary("Combo", "Event")]
    public SerializedDictionary<int, List<SkillAnimationEvent>> Prefabs;

    [Header("Skill Range Object")]
    [Foldout("Generic"), ShowIf("PreCastOn")] public GameObject SkillObjectRangeObject;

    [Header("Casting Skill options")]
    [Foldout("Generic")] public bool BlockWalkWhilePreCasting = true;
    [Foldout("Generic")] public bool BlockDashWhilePreCasting = true;
    [Foldout("Generic")] public bool PreCastOn = true;

    [Header("Skill Parameters")]
    [Foldout("Generic")] public Character SkillCharacter;
    [Foldout("Generic")] public bool Cancelable;

}
