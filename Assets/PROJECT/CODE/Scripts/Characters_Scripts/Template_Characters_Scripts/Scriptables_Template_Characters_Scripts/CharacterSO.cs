using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfExpression { Happy, Sad, Surprised, Horny, Radiant }

[Serializable]
public class CharacterKit {
    public CommonSkillSO BaseAttack, SkillOne, SkillTwo, Dash;
    public UltimateSkillSO Ultimate;
    public PassiveSO Passive;
}

[CreateAssetMenu(menuName = "Characters/ CharactersDescriptions")]
public class CharacterSO : ScriptableObject {
    public string CharacterName;

    [Foldout("Character Selection")] public Sprite CharacterSignature;
    [Foldout("Character Selection")] public Sprite CharacterSelectionImage;
    [Foldout("Character Selection")] public List<CommonSkillSO> CharacterListOfSkills;
    [Foldout("Character Selection")] public List<UltimateSkillSO> CharacterListOfUltimates;

    public Character Character;
    public PassiveSO Passive;
    public CharacterKit InitialKit;
    [SerializedDictionary("Type of Expression", " Sprite")]
    public Dictionary<TypeOfExpression, Sprite> DictionaryOfSprites = new();
}
