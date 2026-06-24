using System.Collections;
using UnityEngine;

public class EnterExitAnimationManager : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] string exitAnimationStateName;
    [SerializeField] float exitAnimationDuration;

    Coroutine _exitAnimationCoroutine;
    WaitForSeconds _exitAnimationDurationWaitForSeconds;
    WaitForSecondsRealtime _exitAnimationDurationWaitForSecondsRealtime;

    private void Awake() {
        _exitAnimationDurationWaitForSeconds = new(exitAnimationDuration);
        _exitAnimationDurationWaitForSecondsRealtime = new(exitAnimationDuration);
    }

    public Coroutine ReturnExitAnimationCoroutine(bool realTime = false) {
        _exitAnimationCoroutine ??= StartCoroutine(ExitAnimationCoroutine(realTime));
        return _exitAnimationCoroutine;
    }

    IEnumerator ExitAnimationCoroutine(bool realTime) {

        switch (realTime) {
            case false:
                anim.Play(exitAnimationStateName);
                yield return _exitAnimationDurationWaitForSeconds;
                break;

            case true:
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                anim.Play(exitAnimationStateName);
                yield return _exitAnimationDurationWaitForSecondsRealtime;
                anim.updateMode = AnimatorUpdateMode.Normal;
                break;
        }

        _exitAnimationCoroutine = null;
    }
}
