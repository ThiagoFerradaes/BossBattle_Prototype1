using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfExpression { Happy, Sad, Surprised, Horny, Radiant}

[Serializable]
public class CharacterKit {
    public CommonSkillSO BaseAttack, SkillOne, SkillTwo, Dash;
    public UltimateSkillSO Ultimate;
    public PassiveSO Passive;
}

[CreateAssetMenu(menuName = "Characters/ CharactersDescriptions")]
public class CharacterSO : ScriptableObject
{
    public Character Character;
    public CharacterKit InitialKit;
    [SerializedDictionary("Type of Expression", " Sprite")]
    public Dictionary<TypeOfExpression, Sprite> DictionaryOfSprites = new();
}
