using UnityEngine;

public class AnimationEvent : StateMachineBehaviour
{
    PlayerMovementManager moveManager;
    PlayerSkillManager skillManager;
    SkillSO skillInfo;
    private bool initialized = false;

    private void Init(Animator animator) {
        
        if (initialized) return;

        moveManager = animator.GetComponentInParent<PlayerMovementManager>();
        skillManager = animator.GetComponentInParent<PlayerSkillManager>();
        if (skillManager != null) skillInfo = skillManager.ReturnCurrentSkill();
        initialized = true;
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        Init(animator);

        if (moveManager != null) moveManager.BlockMovement(true);
        if (skillManager != null) {
            skillManager.BlockAnySkill(true);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (moveManager != null) moveManager.BlockMovement(false);
        if (skillManager != null) skillManager.BlockAnySkill(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
