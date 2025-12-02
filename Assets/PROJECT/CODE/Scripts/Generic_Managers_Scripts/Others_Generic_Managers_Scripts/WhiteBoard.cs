using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public enum Bosses { Kraken, Crab, Thalassia, Voodoo, Birds, Ecdna}

public class CharacterUnlockedInfo {
    public CharacterSO Character;
    public bool IsUnlocked;

    public Dictionary<SkillSlot, List<(SkillSO, bool)>> DictionaryOfUnlockedSkills = new();

    public CharacterUnlockedInfo(CharacterSO character) {
        this.Character = character;
        this.IsUnlocked = false;
        
        foreach(var skill in character.CharacterListOfSkills) {

            if (!DictionaryOfUnlockedSkills.ContainsKey(skill.Slot)) DictionaryOfUnlockedSkills[skill.Slot] = new();

            DictionaryOfUnlockedSkills[skill.Slot].Add((skill, false));
        }
    }
}
public class WhiteBoard : MonoBehaviour
{
    public static WhiteBoard Instance;

    [SerializeField] List<CharacterSO> listOfAllCharacters = new();
    [SerializeField] List<Character> listOfInitialCharactersUnlocked = new();
    [SerializedDictionary("Boss", "Amount Of Phases"), SerializeField]
    SerializedDictionary<Bosses, int> dictionaryOfPhasesByBoss = new();

    List<CharacterUnlockedInfo> _listOfCharactersUnlockedInfo = new();

    Dictionary<Bosses, int> _dictionaryOfUnlockedPhasesByBosses = new();
    Dictionary<BossRewardItem, int> _bossItensInventory = new();

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

            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }

    void CreateListOfCharacter() {
        foreach (var character in listOfAllCharacters) {
            CharacterUnlockedInfo newInfo = new(character);
            _listOfCharactersUnlockedInfo.Add(newInfo);
        }
    }

    #region Getters

    public List<CharacterUnlockedInfo> ReturnListOfUnlockedCharecters() => _listOfCharactersUnlockedInfo;
    public Dictionary<Bosses, int> ReturnListOfUnlockedPhasesByBoss () => _dictionaryOfUnlockedPhasesByBosses;
    #endregion

    #region Setters
    public void UnlockSkill(Character character, SkillSO skillToUnlock) {
        
        foreach(var characterInfo in _listOfCharactersUnlockedInfo) {
            if (characterInfo.Character.Character != character) continue;

            var list = characterInfo.DictionaryOfUnlockedSkills[skillToUnlock.Slot];

            for (int i = 0; i < list.Count; i++) {
                if (list[i].Item1 != skillToUnlock) continue;

                list[i] = (list[i].Item1, true);
            }
        }
    }
    /// <summary>
    /// Add the character to the list of unlocked characters
    /// </summary>
    /// <param name="character"></param>
    public void UnlockCharacter(Character character) {

        foreach(var characterInfo in _listOfCharactersUnlockedInfo) {
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
    /// <summary>
    /// Add the amount of the item to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void RecieveBossItem(BossRewardItem item, int amount) {
        if (_bossItensInventory.ContainsKey(item)) {
            _bossItensInventory[item] += amount;
        }
        else {
            _bossItensInventory[item] = amount; 
        }
    }

    #endregion
}
