using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class CharacterKit {
    public CommonSkillSO BaseAttack, SkillOne, SkillTwo, Dash;
    public UltimateSkillSO Ultimate;
    public PassiveSO Passive;

    public CharacterKit(CharacterKit source) {
        this.BaseAttack = source.BaseAttack;
        this.SkillOne = source.SkillOne;
        this.SkillTwo = source.SkillTwo;
        this.Ultimate = source.Ultimate;
        this.Passive = source.Passive;
        this.Dash = source.Dash;
    }
}

[CreateAssetMenu(menuName = "Characters/ CharactersDescriptions")]
public class CharacterSO : ScriptableObject {

    public string CharacterName;

    [Foldout("Character Selection")] public Sprite CharacterSignature;
    [Foldout("Character Selection")] public Sprite CharacterSelectionImage;
    [Foldout("Character Selection")] public Sprite CharacterIcon;
    [Foldout("Character Selection")] public Sprite CharacterSelectedBackground;
    [Foldout("Character Selection")] public Sprite CharacterLockedMapSprite;
    [Foldout("Character Selection")] public Sprite SelectedCharacterMapSprite;
    [Foldout("Character Selection")] public Sprite UnselectedCharacterMapSprite;
    [Foldout("Character Selection")] public List<SkillSO> CharacterListOfSkills;
    [Foldout("Expressions")]
    [SerializedDictionary("Expression", "Sprite")] public SerializedDictionary<ExpressionTypeDialogue, Sprite> DictionaryOfExpressions;

    public Character Character;
    public GameObject CharacterPrefab;
    public PassiveSO Passive;
    public CharacterKit InitialKit;
}
