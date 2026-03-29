using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPauseManager : MonoBehaviour
{
    bool _isPaused = false;

    public void Pause()
    {
        if (!_isPaused)
        {
            _isPaused = true;
            PauseScreen.Instance.TurnScreenOn();
        }
        else
        {
            _isPaused = false;
            PauseScreen.Instance.TurnScreenOff();
        }

    }
}
