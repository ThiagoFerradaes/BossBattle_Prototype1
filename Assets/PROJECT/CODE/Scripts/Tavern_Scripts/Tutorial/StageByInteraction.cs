using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageByInteraction : TutorialClassBehaviour
{
    public override event Action<bool> OnCompleteTutorialEvent;

    public static event Action OnChangeInteractionRange; 

    private float interactionRange;

    [SerializeField] private UITextLocalizer uiTextLocalizer;
    
    private PlayerActionMap playerActionMap;

    private Transform player;
    
    protected void OnEnable()
    {
        PlayerInteractionManager.OnInteractionDistanceForPublic += UpdateInteraction;
        playerActionMap = new PlayerActionMap();
        playerActionMap.Player.Interaction.started += Interaction;
        playerActionMap.Enable();
        player = PlayerManager.Instance.Player.transform;
        
        uiTextLocalizer.OnTextUpdated += OnAnyButtonPress;
        OnAnyButtonPress(uiTextLocalizer.GetTextString());
        Time();
    }

    protected void OnDisable()
    {
        PlayerInteractionManager.OnInteractionDistanceForPublic -= UpdateInteraction;
        playerActionMap.Player.Interaction.started -= Interaction;
        playerActionMap.Disable();
        uiTextLocalizer.OnTextUpdated -= OnAnyButtonPress;
    }

    private async void Time()
    {
        try
        {
            await Task.Delay(1000);
            if(interactionRange == 0) OnChangeInteractionRange?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    } 
    
    private void UpdateInteraction(float distancePlayer)
    {
        interactionRange = distancePlayer;
    }

    private void Interaction(InputAction.CallbackContext context)
    {
        if (Vector3.Distance(player.position, transform.position) > interactionRange) return;
        
        OnCompleteTutorialEvent?.Invoke(true);
    }
    
    private void OnAnyButtonPress(string text) => uiTextLocalizer.SetTextString(InputActionUtils.ChangeTextForButton(text, playerActionMap.Player.Interaction));
}
