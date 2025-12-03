using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

public class CurrentSelectedCharacterWhiteBoard : MonoBehaviour {
    public static CurrentSelectedCharacterWhiteBoard Instance;

    Character _selectedCharacter = Character.Cyrus;
    CharacterSO _selectedCharacterSO;

    [SerializedDictionary("Character", "Kit"), SerializeField] 
    SerializedDictionary<Character, CharacterKit> charactersCurrentSkills = new();


    public event Action<CharacterSO> OnSelectedCharacterChanged;

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
    /// Return the Selected Playable Character Info
    /// </summary>
    /// <returns></returns>
    public CharacterSO ReturnSelectedCharacterSO() => _selectedCharacterSO;
    /// <summary>
    /// Return the first skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public CommonSkillSO ReturnSkillOne(Character character) => charactersCurrentSkills[character].SkillOne;
    /// <summary>
    /// Return the second skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public CommonSkillSO ReturnSkillTwo(Character character) => charactersCurrentSkills[character].SkillTwo;
    /// <summary>
    /// Return the ultimate skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public UltimateSkillSO ReturnUltimate(Character character) => charactersCurrentSkills[character].Ultimate;
    /// <summary>
    /// Return the dash skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public CommonSkillSO ReturnDash(Character character) => charactersCurrentSkills[character].Dash;
    /// <summary>
    /// Return the base attack skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public CommonSkillSO ReturnBaseAttack(Character character) => charactersCurrentSkills[character].BaseAttack;
    /// <summary>
    /// Return the passive skill from the current Selected Playable Character
    /// </summary>
    /// <returns></returns>
    public PassiveSO ReturnPassive(Character character) => charactersCurrentSkills[character].Passive;

    #endregion

    #region Setter

    /// <summary>
    /// Set the current Selected Playable Character to a new character
    /// </summary>
    /// <param name="newSelectedCharacter"></param>
    public void SetSelectedCharacter(CharacterSO newSelectedCharacter) { 
        _selectedCharacter = newSelectedCharacter.Character;
        _selectedCharacterSO = newSelectedCharacter;

        if (!charactersCurrentSkills.ContainsKey(newSelectedCharacter.Character)) {
            charactersCurrentSkills[newSelectedCharacter.Character] = newSelectedCharacter.InitialKit;
        }

        OnSelectedCharacterChanged?.Invoke(newSelectedCharacter);
    }

    /// <summary>
    /// Set first skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetFirstSkill(CommonSkillSO newSkill, Character character) {
        if (!charactersCurrentSkills.ContainsKey(character) || charactersCurrentSkills[character].SkillOne == newSkill) return;

        charactersCurrentSkills[character].SkillOne = newSkill;
    }
    /// <summary>
    /// Set second skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetSecondSkill(CommonSkillSO newSkill, Character character) {
        if (!charactersCurrentSkills.ContainsKey(character) || charactersCurrentSkills[character].SkillTwo == newSkill) return;

        charactersCurrentSkills[character].SkillTwo = newSkill;
    }
    /// <summary>
    /// Set ultimate skill of the selected character, if the skill is not from the current character it wont work
    /// </summary>
    /// <param name="newSkill"></param>
    public void SetUltimateSkill(UltimateSkillSO newSkill, Character character) {
        if (!charactersCurrentSkills.ContainsKey(character) || charactersCurrentSkills[character].Ultimate == newSkill) return;

        charactersCurrentSkills[character].Ultimate = newSkill;
    }
    #endregion
}


