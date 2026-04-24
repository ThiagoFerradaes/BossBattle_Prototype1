using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Bosses { Kraken, Crab, Thalassia, Voodoo, Birds, Ecdna }

public enum SkillType { Classic, Alternative }
[Serializable]
public class SkillUnlockedInfo {
    public SkillSO Skill;
    public bool IsUnlocked;
    public SkillType Type;

    public SkillUnlockedInfo(SkillSO skill) {
        this.Skill = skill;
        this.Type = skill.SkillType;
        this.IsUnlocked = Type == SkillType.Classic ? true : false;
    }
}
[Serializable]
public class CharacterUnlockedInfo {
    public CharacterSO Character;
    public bool IsUnlocked;

    [SerializedDictionary("Slot", "Info")]
    public SerializedDictionary<SkillSlot, List<SkillUnlockedInfo>> DictionaryOfUnlockedSkills = new();

    public CharacterUnlockedInfo(CharacterSO character) {
        this.Character = character;
        this.IsUnlocked = false;

        if (character.CharacterListOfSkills == null) return;

        foreach (var skill in character.CharacterListOfSkills) {

            if (!DictionaryOfUnlockedSkills.ContainsKey(skill.Slot)) DictionaryOfUnlockedSkills[skill.Slot] = new();

            SkillUnlockedInfo newInfo = new(skill);
            DictionaryOfUnlockedSkills[skill.Slot].Add((newInfo));
        }
    }
}
public class WhiteBoard : MonoBehaviour {
    public static WhiteBoard Instance;

    [SerializedDictionary("Character", "Info"), SerializeField] SerializedDictionary<Character, CharacterSO> listOfAllCharacters = new();
    [SerializedDictionary("Character", "Info"), SerializeField] SerializedDictionary<Character, List<SkillSO>> listOfSkillsToUnlock = new();
    [SerializeField] List<Character> listOfInitialCharactersUnlocked = new();
    [SerializedDictionary("Boss", "Amount Of Phases"), SerializeField]
    SerializedDictionary<Bosses, int> dictionaryOfPhasesByBoss = new();

    [SerializeField] List<CharacterUnlockedInfo> _listOfCharactersUnlockedInfo = new();

    Dictionary<Bosses, int> _dictionaryOfUnlockedPhasesByBosses = new();

    public static event Action<SkillSlot, float> OnCooldownSet;
    public static event Action<SkillSlot, int, bool> OnChargesSet;
    public static event Action<SkillSlot, int, bool> OnChargesChange;

    
    public void SetCooldown(SkillSlot slot, float cooldown) => OnCooldownSet?.Invoke(slot, cooldown);
    
    public void SetCharges(SkillSlot slot, int charges, bool hasCharges) => OnChargesSet?.Invoke(slot, charges, hasCharges);
    public void SetChargesChange(SkillSlot slot, int charges, bool hasCharges) => OnChargesChange?.Invoke(slot, charges, hasCharges);
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;

            CreateListOfCharacter();

            foreach (var character in listOfInitialCharactersUnlocked) {
                UnlockCharacter(character);

            }
            foreach (var bossPhase in dictionaryOfPhasesByBoss) {
                UnlockPhase(bossPhase.Key, bossPhase.Value);
            }
            foreach (var skillToUnlock in listOfSkillsToUnlock) {
                foreach (var skill in skillToUnlock.Value) {
                    UnlockSkill(skillToUnlock.Key, skill);
                }
            }

            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }


    private void Start() {
        CurrentSelectedCharacterWhiteBoard.Instance.SetSelectedCharacter(listOfAllCharacters[Character.Cyrus]);

    }

    void CreateListOfCharacter() {
        foreach (var character in listOfAllCharacters.Values) {
            CharacterUnlockedInfo newInfo = new(character);
            _listOfCharactersUnlockedInfo.Add(newInfo);
        }
    }

    #region Getters

    public List<CharacterUnlockedInfo> ReturnListOfUnlockedCharacters() => _listOfCharactersUnlockedInfo;
    public Dictionary<Bosses, int> ReturnListOfUnlockedPhasesByBoss() => _dictionaryOfUnlockedPhasesByBosses;

    public List<SkillUnlockedInfo> ReturnCurrentCharacterSkillsBySlot(SkillSlot slot) {
        foreach (var character in _listOfCharactersUnlockedInfo) {
            if (character.Character != CurrentSelectedCharacterWhiteBoard.Instance.ReturnSelectedCharacterSO()) continue;

            return character.DictionaryOfUnlockedSkills[slot];
        }

        return null;
    }
    #endregion

    #region Setters
    public void UnlockSkill(Character character, SkillSO skillToUnlock) {

        foreach (var characterInfo in _listOfCharactersUnlockedInfo) {
            if (characterInfo.Character.Character != character) continue;

            var list = characterInfo.DictionaryOfUnlockedSkills[skillToUnlock.Slot];

            for (int i = 0; i < list.Count; i++) {
                if (list[i].Skill != skillToUnlock) continue;

                list[i].IsUnlocked = true;
            }
        }
    }
    /// <summary>
    /// Add the character to the list of unlocked characters
    /// </summary>
    /// <param name="character"></param>
    public void UnlockCharacter(Character character) {

        foreach (var characterInfo in _listOfCharactersUnlockedInfo) {
            if (characterInfo.Character.Character == character) {
                characterInfo.IsUnlocked = true;
            }
        }
    }
    /// <summary>
    /// Add the phase to the list of unlocked phases
    /// </summary>
    /// <param name="phase"></param>
    public void UnlockPhase(Bosses boss, int amountOfPhasesUnlocked) {
        _dictionaryOfUnlockedPhasesByBosses[boss] = amountOfPhasesUnlocked;
    }


    #endregion
}
