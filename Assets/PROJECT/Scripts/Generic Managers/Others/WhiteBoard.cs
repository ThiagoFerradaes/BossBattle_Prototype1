using System.Collections.Generic;
using UnityEngine;

public enum Phases { 
    KrakenOne, KrakenTwo, KrakenThree, KrakenFour, KrakenFive, 
    SecondOne, SecondTwo, SecondThree, SecondFour, SecondFive,
    ThirdOne, ThirdTwo, ThirdThree, ThirdFour, ThirdFive,
    FourthOne, FourthTwo, FourthThree, FourthFour, FourthFive,
    FifthOne, FifthTwo, FifthThree, FifthFour, FifthFive,
    FinalBossOne, FinalBossTwo
}
public class WhiteBoard : MonoBehaviour
{
    public static WhiteBoard Instance;

    List<Character> _listOfUnlockedCharacter = new();
    List<Phases> _listOfUnlockedPhases = new();


    private void Awake() {
        if (Instance == null) {
            Instance = this;
            UnlockCharacter(Character.WeaponMaster);
            UnlockPhase(Phases.KrakenOne);
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(Instance);
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

    #endregion
}
