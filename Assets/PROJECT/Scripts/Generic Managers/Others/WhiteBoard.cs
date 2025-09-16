using System.Collections.Generic;
using UnityEngine;

public enum Phases { 
    KrakenOne, KrakenTwo, KrakenThree, KrakenFour, KrakenFive, 
    SecondOne, SecondTwo, SecondThree, SecondFour, SecondFive,
    ThirdOne, ThirdTwo, ThirdThree, ThirdFour, ThirdFive,
    FourthOne, FourthTwo, FourthThree, FourthFour, FourthFive,
    FifthOne, FifthTwo, FifthThree, FifthFour, FifthFive,
    FinalBossOne, FinalBossTwo, Null
}
public class WhiteBoard : MonoBehaviour
{
    public static WhiteBoard Instance;

    List<Character> _listOfUnlockedCharacter = new();
    List<Phases> _listOfUnlockedPhases = new();

    Dictionary<BossRewardItem, int> _bossItensInventory = new();


    private void Awake() {
        if (Instance == null) {
            Instance = this;
            UnlockCharacter(Character.WeaponMaster);
            UnlockCharacter(Character.Bastian);
            UnlockCharacter(Character.Lilian);
            UnlockPhase(Phases.KrakenOne);
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }
    }


    #region Getters

    public List<Character> ReturnListOfUnlockedCharecters() => _listOfUnlockedCharacter;
    public List<Phases> ReturnListOfUnlockedPhases() => _listOfUnlockedPhases;

    #endregion

    #region Setters

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
    public void UnlockPhase(Phases phase) {
        if (!_listOfUnlockedPhases.Contains(phase)) {
            _listOfUnlockedPhases.Add(phase);
        }
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
