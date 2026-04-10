using System.Collections.Generic;
using UnityEngine;

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
    public void ChangeAnimation(Animator anim, AnimationClip newClip, bool isLoop = false, float crossFase = 0.05f, int layer = 0) {

        var controller = GetOverrideController(anim);

        if (isLoop) {
            controller["Default 1"] = newClip;

            anim.CrossFade("LoopAnimation", crossFase, layer, 0f);
        }
        else {
            controller["Default"] = newClip;

            anim.CrossFade("OneShotAnimation", crossFase, layer, 0f);
        }
    }
    public void ReturnToIdle(Animator anim) {
        anim.CrossFade("Idle", 0.05f, 0, 0f);
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
