using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfExpression { Happy, Sad, Surprised, Horny, Radiant}

[Serializable]
public class CharacterKit {
    public SkillSO BaseAttack, SkillOne, SkillTwo, Ultimate, Dash;
    public PassiveSO Passive;
}

[CreateAssetMenu(menuName = "CharactersDescriptions")]
public class CharacterSO : ScriptableObject
{
    public Character Character;
    public CharacterKit InitialKit;
    [SerializedDictionary("Type of Expression", " Sprite")]
    public Dictionary<TypeOfExpression, Sprite> DictionaryOfSprites = new();
}
