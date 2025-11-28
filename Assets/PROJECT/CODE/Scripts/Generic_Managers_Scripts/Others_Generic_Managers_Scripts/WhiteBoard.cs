using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public enum Bosses { Kraken, Crab, Thalassia, Voodoo, Birds, Ecdna}
public class WhiteBoard : MonoBehaviour
{
    public static WhiteBoard Instance;

    [SerializeField] List<Character> listOfInitialCharactersUnlocked = new();
    [SerializedDictionary("Boss", "Amount Of Phases"), SerializeField]
    SerializedDictionary<Bosses, int> dictionaryOfPhasesByBoss = new();

    List<Character> _listOfUnlockedCharacter = new();
    Dictionary<Bosses, int> _dictionaryOfUnlockedPhasesByBosses = new();
    Dictionary<BossRewardItem, int> _bossItensInventory = new();
    Dictionary<Character, List<SkillSO>> _dictionaryOfUnlockedSkills = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;

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


    #region Getters

    public List<Character> ReturnListOfUnlockedCharecters() => _listOfUnlockedCharacter;
    public Dictionary<Bosses, int> ReturnListOfUnlockedPhasesByBoss () => _dictionaryOfUnlockedPhasesByBosses;
    #endregion

    #region Setters
    public void UnlockSkill(Character character, SkillSO skillToUnlock) {
        if (!_dictionaryOfUnlockedSkills[character].Contains(skillToUnlock)) {
            _dictionaryOfUnlockedSkills[character].Add(skillToUnlock);
        }
    }
    /// <summary>
    /// Add the character to the list of unlocked characters
    /// </summary>
    /// <param name="character"></param>
    public void UnlockCharacter(Character character) {
        if (!_listOfUnlockedCharacter.Contains(character)) {
            _listOfUnlockedCharacter.Add(character);
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
