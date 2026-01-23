using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StageByMove : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;
    
    private byte moveInput;

    [SerializeField] private UITextLocalizer uiTextLocalizer;
    
    [SerializeField] private GameObject canvasSImage, canvasWImage,canvasAImage,canvasDImage;
    
    public void Move(InputAction.CallbackContext context)
    {
        var cont = context.ReadValue<Vector2>();

        switch (cont.x)
        {
            case > 0:
            {
                if (moveInput != 1 && moveInput != 3 && moveInput != 5 && moveInput != 7 && moveInput != 9 &&
                    moveInput != 11 && moveInput != 13 && moveInput != 15)
                {
                    moveInput += 1;
                    canvasAImage.SetActive(true);
                }

                break;
            }
            case < 0:
            {
                if (moveInput != 2 && moveInput != 3 && moveInput != 6 && moveInput != 7 && moveInput != 11 &&
                    moveInput != 14 && moveInput != 10 && moveInput != 15)
                {
                    moveInput += 2;
                    canvasDImage.SetActive(true);
                }

                break;
            }
        }

        switch (cont.y)
        {
            case > 0:
            {
                if (moveInput != 4 && moveInput != 5 && moveInput != 6 && moveInput != 7 && moveInput != 12 &&
                    moveInput != 13 && moveInput != 14 && moveInput != 15)
                {
                    moveInput += 4;
                    canvasWImage.SetActive(true);
                }

                break;
            }
            case < 0:
            {
                if (moveInput != 8 && moveInput != 9 && moveInput != 10 && moveInput != 12 && moveInput != 14 &&
                    moveInput != 15 && moveInput != 13 && moveInput != 11)
                {
                    moveInput += 8;
                    canvasSImage.SetActive(true);
                }

                break;
            }
        }
        
        
        if (moveInput == 15) OnCompleteTutorialEvent?.Invoke(true);
    }
    
    private PlayerActionMap playerActionMap;
    
    private void OnEnable()
    {
        playerActionMap = new PlayerActionMap();
        uiTextLocalizer.OnTextUpdated += OnAnyButtonPress;

        OnAnyButtonPress(uiTextLocalizer.GetTextString());
    }

    private void OnDisable()
    {
        playerActionMap.Disable();
        uiTextLocalizer.OnTextUpdated -= OnAnyButtonPress;
    }
    
    private void OnAnyButtonPress(string text)
    {
        Debug.Log(text);
        var replace = text.Replace("<><>", InputActionUtils.GetCompositeKeys(playerActionMap.Player.Move));
        Debug.Log(replace);
        uiTextLocalizer.SetTextString(replace);
    }

}
