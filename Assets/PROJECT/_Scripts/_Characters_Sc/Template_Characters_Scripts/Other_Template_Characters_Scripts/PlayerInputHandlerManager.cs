using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InteractionManager), typeof(PlayerMovementManager))]
public class PlayerInputHandlerManager : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private PlayerMovementManager moveManager;

    bool _canInput = true;

    public static event Action<SkillSlot, InputAction.CallbackContext> OnSkillInputPerformed;

    private void Awake()
    {
        _canInput = true;

        if (interactionManager == null) interactionManager = GetComponent<InteractionManager>();
        if (moveManager == null) moveManager = GetComponent<PlayerMovementManager>();
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput() || !ctx.performed) return;

        interactionManager.HandleInteraction(this);
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        moveManager.DetectRotation(ctx.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {

        if (CheckIfCantInput(false) || !ctx.performed) return;

        PauseScreen.Instance.Pause();
    }

    public void OnBaseAttack(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        OnSkillInputPerformed?.Invoke(SkillSlot.BaseAttack, ctx);

    }

    public void OnSkillOne(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        OnSkillInputPerformed?.Invoke(SkillSlot.SkillOne, ctx);
    }
    public void OnSkillTwo(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        OnSkillInputPerformed?.Invoke(SkillSlot.SkillTwo, ctx);
    }
    public void OnUltimate(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        OnSkillInputPerformed?.Invoke(SkillSlot.Ultimate, ctx);
    }
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        OnSkillInputPerformed?.Invoke(SkillSlot.Dash, ctx);
    }
    bool CheckIfCantInput(bool withTime = true)
    {
        if (withTime) return Time.timeScale == 0 || !_canInput;
        else return !_canInput;
    }


    public void SetCanInput(bool canInputValue)
    {
        _canInput = canInputValue;

        HandleMovePermissions();
    }

    void HandleMovePermissions()
    {
        moveManager.BlockMovement(!_canInput);
    }
}
