using NaughtyAttributes;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InteractionManager), typeof(PlayerMovementManager), typeof(PlayerPauseManager))]
public class PlayerInputHandlerManager : MonoBehaviour
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private PlayerMovementManager moveManager;
    [SerializeField] private PlayerPauseManager pauseManager;
#pragma warning disable CS0414
    [SerializeField] private bool hasSkills = true;
#pragma warning restore CS0414
    [SerializeField, ShowIf("hasSkills"), AllowNesting] private PlayerSkillManager skillManager;

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

        skillManager.BaseAttack(ctx);

    }

    public void OnSkillOne(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        skillManager.SkillOne(ctx);
    }
    public void OnSkillTwo(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        skillManager.SkillTwo(ctx);
    }
    public void OnUltimate(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        skillManager.Ultimate(ctx);
    }
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (CheckIfCantInput()) return;

        skillManager.Dash(ctx);
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
