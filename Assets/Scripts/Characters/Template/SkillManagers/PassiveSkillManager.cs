using UnityEngine;

public class PassiveSkillManager : MonoBehaviour
{
    protected GameObject parent;
    public virtual void OnStart(PassiveSO skill, GameObject parent) {
        this.parent = parent;
    }
}
