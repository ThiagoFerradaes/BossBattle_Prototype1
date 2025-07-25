using UnityEngine;

public abstract class SkillObjectManager : MonoBehaviour
{
    public abstract void OnStart(SkillSO skill, PlayerSkillManager skillManager, Animator anim, SkillSlot slot);
}
