using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SkillAnimationEvent {
    public float TimeToSpawnPreFab;
    public TypeOfSkillPrefab PrefabType;
    [ShowIf("PrefabType", TypeOfSkillPrefab.VFX), AllowNesting]
    public VFXAtributes VFXAtribute;
    [ShowIf("PrefabType", TypeOfSkillPrefab.VFX), AllowNesting]
    public GameObject PreFab;
    public Vector3 PreFabPosition;
}

public class SkillSO : ScriptableObject {
    [Header("Skill Description")]
    [Foldout("Generic")] public string SkillName;
    [Foldout("Generic"), TextArea(3, 10)] public string SkillShortDescription;
    [Foldout("Generic"), TextArea(3, 10)] public string SkillLongDescription;
    [Foldout("Generic")] public Sprite SkillSpriteIcon;

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
    [Foldout("Generic")] public SkillSlot Slot;
    [Foldout("Generic")] public SkillType SkillType;

}
