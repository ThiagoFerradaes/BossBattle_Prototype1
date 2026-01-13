using System;
using UnityEngine.InputSystem;

public class StageByMove : TutorialClassBehaviour
{
    public override event Action OnCompleteTutorialEvent;

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started) OnCompleteTutorialEvent?.Invoke();
    }
}
