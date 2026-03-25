using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageByMove : TutorialClassBehaviour
{
    #region Event

    public override event Action<bool> OnCompleteTutorialEvent;

    #endregion

    #region SerializeField

    [SerializeField] protected UITextLocalizer uiTextLocalizer;
    
    [SerializeField] private GameObject canvasSImage, canvasWImage,canvasAImage,canvasDImage;

    [SerializeField] private Transform playerCanInDisPosition;

    [SerializeField] private String playerAnimation;
    
    private PlayerMovementManager playerMovementManager;
    #endregion

    #region private Fields

    private PlayerActionMap playerActionMap;
    
    MoveFlags moveInput;

    #endregion

    #region Flags

    [Flags]
    enum MoveFlags : byte
    {
        Up    = 1 << 0, // 1
        Down  = 1 << 1, // 2
        Left  = 1 << 2, // 4
        Right = 1 << 3  // 8
    }

    private void AddFlag(MoveFlags flag, GameObject canvasImage)
    {
        if ((moveInput & flag) != 0)
            return;

        moveInput |= flag;
        canvasImage.SetActive(true);
    }
    
    #endregion

    #region Input

    public void Move(InputAction.CallbackContext context)
    {
        var inputBinding = context.action.GetBindingForControl(context.control);

        if (inputBinding != null)
            switch (inputBinding.Value.name)
            {
                case "up":
                    AddFlag(MoveFlags.Up, canvasWImage);
                    break;
                case "down":
                    AddFlag(MoveFlags.Down, canvasSImage);
                    break;
                case "left":
                    AddFlag(MoveFlags.Left, canvasDImage);
                    break;
                case "right":
                    AddFlag(MoveFlags.Right, canvasAImage);
                    break;
            }

        if (moveInput == (MoveFlags.Up | MoveFlags.Down | MoveFlags.Left | MoveFlags.Right))
        {
            OnCompleteTutorialEvent?.Invoke(true);
        }
    }

    #endregion
    
    #region Unity Callbacks
    
    protected void OnEnable()
    {
        playerActionMap = new PlayerActionMap();
        uiTextLocalizer.OnTextUpdated += OnAnyButtonPress;

        OnAnyButtonPress(uiTextLocalizer.GetTextString());
        
        //set player Transform
        var player = PlayerManager.Instance.Player.transform;
        
        playerMovementManager = player.GetComponent<PlayerMovementManager>();
        playerMovementManager.enabled = false;
        
        player.position = playerCanInDisPosition.position;
        player.rotation = playerCanInDisPosition.rotation;
        
        //set player animation //and position
        var animator = player.GetComponentInChildren<Animator>();
        
        StartCoroutine(PlayAndWaitForAnim(playerAnimation, animator));
    }

    public IEnumerator PlayAndWaitForAnim(string stateName, Animator animator)
    {
        animator.Play(stateName);
        // Wait until we enter the correct state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // Wait until the current state is done playing (normalizedTime >= 1.0f)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        playerMovementManager.enabled = true;
    }
    
    protected void OnDisable()
    {
        playerActionMap.Disable();
        uiTextLocalizer.OnTextUpdated -= OnAnyButtonPress;
    }
    
    #endregion
    
    #region Private Methods
    
    private void OnAnyButtonPress(string text) => uiTextLocalizer.SetTextString(InputActionUtils.ChangeTextForButton(text, playerActionMap.Player.Move));
    
    #endregion
}