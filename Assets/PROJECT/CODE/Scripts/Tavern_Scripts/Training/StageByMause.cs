using System;
using UnityEngine.InputSystem;

public class StageByMause : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started) OnCompleteTutorialEvent?.Invoke(true);
    }
}
