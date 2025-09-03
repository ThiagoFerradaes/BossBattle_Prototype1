using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfSkillPrefab { Hitbox, VFX, PreCastRange}
[System.Serializable]
public class SkillAnimationEvent {
    public float TimeToSpawnPreFab;
    public float PrefabDuration;
    public string PreFabName;
    public TypeOfSkillPrefab PrefabType;
    public GameObject PreFab;
    public Vector3 PreFabPosition;
}

public enum Tags { Enemy, Player }
public abstract class CommonSkillSO : SkillSO
{


}
