using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[System.Serializable]
public class SkillAnimationEvent {
    public float TimeToSpawnPreFab;
    public TypeOfSkillPrefab PrefabType;
    public GameObject PreFab;
    public Vector3 PreFabPosition;
    [ShowIf("PrefabType", TypeOfSkillPrefab.VFX), AllowNesting] public VFXAtributes VFXAtribute;
}
[System.Serializable]
public class UIDescriptionInfo {
    public LocalizedString SkillName;
    public LocalizedString SkillShortDescription;
    public LocalizedString SkillLongDescription;
    public Sprite UISkillSpriteIcon;
    public Sprite MapSkillSpriteIcon;
    public Sprite MapSkillSelectedSpriteIcon;
    public Sprite MapLockSkillSpriteIcon;
}

public class SkillSO : ScriptableObject {
    [Header("Skill Description")]

    [Foldout("SkillSO")]
    public bool HasMapDescription = true;

    [Foldout("SkillSO"), ShowIf(nameof(HasMapDescription)), AllowNesting]
    public UIDescriptionInfo MapDescriptionInfo;

    [Header("Skill Manager")]
    [Foldout("SkillSO")] public SkillObjectManager SkillManagerObject;

    [Header("Skill Animations")]
    [Foldout("SkillSO")] public List<AnimationInfo> ListOfAnimationsInfo;

    [Header("Skill Prefabs")]
    [Foldout("SkillSO"), SerializedDictionary("Combo", "Event")]
    public SerializedDictionary<int, List<SkillAnimationEvent>> Prefabs;

    [Header("Skill Range Object")]
    [Foldout("SkillSO"), ShowIf("PreCastOn")] public GameObject SkillObjectRangeObject;

    [Header("Casting Skill options")]
    [Foldout("SkillSO")] public bool BlockWalkWhilePreCasting = true;
    [Foldout("SkillSO")] public bool BlockDashWhilePreCasting = true;
    [Foldout("SkillSO")] public bool PreCastOn = true;

    [Header("Skill Parameters")]
    [Foldout("SkillSO")] public Character SkillCharacter;
    [Foldout("SkillSO")] public bool Cancelable;
    [Foldout("SkillSO")] public SkillSlot Slot;
    [Foldout("SkillSO")] public SkillType SkillType;

}
