using UnityEngine;

[CreateAssetMenu(menuName = "Skills / BaseAttack")]
public class BaseAttackSO : SkillSO {

    [Header("Animation")]
    [SerializeField] string animationTriggerName;

    public override void UseSKill(Animator anim) {
        anim.SetTrigger(animationTriggerName);
    }
}
