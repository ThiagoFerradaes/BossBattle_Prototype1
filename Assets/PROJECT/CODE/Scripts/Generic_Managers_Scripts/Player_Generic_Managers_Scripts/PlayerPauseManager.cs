using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPauseManager : MonoBehaviour
{
    bool _isPaused = false;

    public void Pause()
    {

        //if (!_isPaused)
        //{
        //    _isPaused = true;
        //    PauseScreen.Instance.TurnScreenOn();
        //    PauseScreen.Instance.OnDespause += Unpause;
        //}
        //else
        //{
        //    _isPaused = false;
        //    PauseScreen.Instance.TurnScreenOff();
        //    PauseScreen.Instance.OnDespause -= Unpause;
        //}

    }

    //void Unpause() {
    //    _isPaused = false;
    //    PauseScreen.Instance.OnDespause -= Unpause;
    //}

    //private void OnDestroy() {
    //    PauseScreen.Instance.OnDespause -= Unpause;
    //}
}
