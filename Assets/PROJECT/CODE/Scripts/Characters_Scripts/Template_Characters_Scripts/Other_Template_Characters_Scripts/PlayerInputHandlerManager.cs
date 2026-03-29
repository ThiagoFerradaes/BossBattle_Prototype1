using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InteractionManager), typeof(PlayerMovementManager), typeof(PlayerPauseManager))]
public class PlayerInputHandlerManager : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private PlayerMovementManager moveManager;
    [SerializeField] private PlayerPauseManager pauseManager;

    bool _canInput = true;

    private void Awake()
    {
        _canInput = true;

        if (interactionManager == null) interactionManager = GetComponent<InteractionManager>();
        if (moveManager == null) moveManager = GetComponent<PlayerMovementManager>();
        if (pauseManager == null) pauseManager = GetComponent<PlayerPauseManager>();
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (CheckIfCanInput() || !ctx.performed) return;

        interactionManager.HandleInteraction(this);
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (CheckIfCanInput()) return;

        moveManager.DetectRotation(ctx.ReadValue<Vector2>());
    }

    public void OnWalk(InputAction.CallbackContext ctx)
    {
        if (CheckIfCanInput()) return;

        moveManager.SetWalkInputs(ctx.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (CheckIfCanInput(false) || !ctx.performed) return;

        pauseManager.Pause();
    }

    bool CheckIfCanInput(bool withTime = true)
    {
        if (withTime) return Time.timeScale == 0 || !_canInput;
        else return !_canInput;
    }






    public void SetCanInput(bool canInputValue)
    {
        _canInput = canInputValue;

        ResetValueWhenCantInput();
    }

    void ResetValueWhenCantInput()
    {
        if (_canInput) return;

        moveManager.ResetWalkInputs();
    }
}
