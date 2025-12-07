using UnityEngine;

public class PassiveSO : ScriptableObject
{
    [Header("Passive Manager")]
    public PassiveSkillManager PassiveManager;
    public string PassiveName;
    [TextArea(3, 10)] public string ShortDescription;
    [TextArea(3, 10)] public string LongDescription;
    public Sprite PassiveIcon;
    public Character PassiveCharacter;
}
