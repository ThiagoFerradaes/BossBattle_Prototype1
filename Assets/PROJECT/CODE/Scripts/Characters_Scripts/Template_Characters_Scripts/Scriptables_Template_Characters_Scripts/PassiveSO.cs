using UnityEngine;
using UnityEngine.Localization;

public class PassiveSO : ScriptableObject
{
    [Header("Passive Manager")]
    public PassiveSkillManager PassiveManager;
    public LocalizedString PassiveName;
    public LocalizedString ShortDescription;
    public LocalizedString LongDescription;
    public Sprite PassiveIcon;
    public Character PassiveCharacter;
}
