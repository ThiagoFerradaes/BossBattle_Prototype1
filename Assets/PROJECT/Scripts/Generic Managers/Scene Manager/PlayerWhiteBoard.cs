using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerWhiteBoard : MonoBehaviour {
    public static PlayerWhiteBoard Instance;

    Character _selectedCharacter = Character.WeaponMaster;

    [SerializedDictionary("Character", "Kit"), SerializeField] 
    SerializedDictionary<Character, CharacterKit> _charactersCurrentSkills = new();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(this);
        }   
    }


    #region Getters

    /// <summary>
    /// Return the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public Character ReturnSelectedCharacter() => _selectedCharacter;
    /// <summary>
    /// Return the first skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public SkillSO ReturnSkillOne(Character character) => _charactersCurrentSkills[character].SkillOne;
    /// <summary>
    /// Return the second skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public SkillSO ReturnSkillTwo(Character character) => _charactersCurrentSkills[character].SkillTwo;
    /// <summary>
    /// Return the ultimate skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public SkillSO ReturnUltimate(Character character) => _charactersCurrentSkills[character].Ultimate;
    /// <summary>
    /// Return the dash skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public SkillSO ReturnDash(Character character) => _charactersCurrentSkills[character].Dash;
    /// <summary>
    /// Return the base attack skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public SkillSO ReturnBaseAttack(Character character) => _charactersCurrentSkills[character].BaseAttack;
    /// <summary>
    /// Return the passive skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public PassiveSO ReturnPassive(Character character) => _charactersCurrentSkills[character].Passive;

    #endregion

    #region Setter

    /// <summary>
    /// Set the current Selected Playable Character to a new character
    /// </summary>
    /// <param name="newSelectedCharacter"></param>
    public void SetSelectedCharacter(CharacterSO newSelectedCharacter) { 
        _selectedCharacter = newSelectedCharacter.Character; 

        if (!_charactersCurrentSkills.ContainsKey(newSelectedCharacter.Character)) {
            _charactersCurrentSkills[newSelectedCharacter.Character] = newSelectedCharacter.InitialKit;
        }
    }

    /// <summary>
    /// Set first skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetFirstSkill(SkillSO newSkill, Character character) {
        if (!_charactersCurrentSkills.ContainsKey(character) || _charactersCurrentSkills[character].SkillOne == newSkill) return;

        _charactersCurrentSkills[character].SkillOne = newSkill;
    }
    /// <summary>
    /// Set second skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetSecondSkill(SkillSO newSkill, Character character) {
        if (!_charactersCurrentSkills.ContainsKey(character) || _charactersCurrentSkills[character].SkillTwo == newSkill) return;

        _charactersCurrentSkills[character].SkillTwo = newSkill;
    }
    /// <summary>
    /// Set ultimate skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetUltimateSkill(SkillSO newSkill, Character character) {
        if (!_charactersCurrentSkills.ContainsKey(character) || _charactersCurrentSkills[character].Ultimate == newSkill) return;

        _charactersCurrentSkills[character].Ultimate = newSkill;
    }
    #endregion
}


