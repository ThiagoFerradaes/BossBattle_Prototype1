using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationInfo {
    public AnimationClip AnimationClip;
    public int AnimationLayer;
    public float AnimationBaseSpeed = 1;
    public bool Loop = false;
    [Range(0, 1), HideIf("Loop"), AllowNesting] public float AnimationExitTime = 1;
}
public class AnimationManager : MonoBehaviour {

    #region Variables

    public static AnimationManager Instance;

    Dictionary<Animator, AnimatorOverrideController> _overrideControllers = new();

    #endregion

    #region Initialize
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Animations
    public void ChangeAnimation(Animator anim, AnimationInfo animInfo, float extraAnimationSpeed = 1, float crossFase = 0.05f) {

        var controller = GetOverrideController(anim);

        if (animInfo.Loop) {
            controller["Default 1"] = animInfo.AnimationClip;

            anim.CrossFade("LoopAnimation", crossFase, animInfo.AnimationLayer, 0f);
        }
        else {
            controller["Default"] = animInfo.AnimationClip;

            anim.CrossFade("OneShotAnimation", crossFase, animInfo.AnimationLayer, 0f);
        }

        anim.SetFloat("AnimationSpeed", animInfo.AnimationBaseSpeed * extraAnimationSpeed);
    }
    public void ReturnToIdle(Animator anim) {
        anim.CrossFade("Idle", 0.05f, 0, 0f);
    }
    public void SetIdleAnimation(Animator anim, AnimationClip idleClip) {
        var controller = GetOverrideController(anim);
        controller["Idle"] = idleClip;
    }

    public void ResetAnimationSpeed(Animator anim) {
        anim.SetFloat("AnimationSpeed", 1);
    }
    #endregion

    #region PoolingRegion
    AnimatorOverrideController GetOverrideController(Animator anim) {
        if (_overrideControllers.TryGetValue(anim, out var controller)) return controller;

        controller = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = controller;
        _overrideControllers[anim] = controller;
        return controller;
    }
    #endregion

}
