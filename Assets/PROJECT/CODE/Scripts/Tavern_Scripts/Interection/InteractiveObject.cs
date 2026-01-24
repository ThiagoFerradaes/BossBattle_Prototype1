using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveObject : MonoBehaviour
{
    #region Events
    
    public static event Action OnChangeInteractionRange; 
    
    #endregion

    #region public Variables

    public InteractionSO interaction;

    #endregion

    #region Serializable Variables

    [SerializeField] private TMP_Text uiInteraction;

    [SerializeField] private float radius = 0.5f;
    
    #endregion
    
    #region private Variables

    private Transform playerTransform;

    private float distance;

    private bool activeCanva;

    private PlayerActionMap playerActionMap;
    
    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        playerTransform = PlayerManager.Instance.Player.transform;
        PlayerInteractionManager.OnInteractionDistanceForPublic += UpdateInteraction;
        playerActionMap = new PlayerActionMap();
        Time();
    }   

    private void OnDisable()
    {
        PlayerInteractionManager.OnInteractionDistanceForPublic -= UpdateInteraction;
        playerActionMap.Disable();
    }

    #endregion

    #region private Methods

    private async void Time()
    {
        try
        {
            await Task.Delay(1000);
            if(distance == 0) OnChangeInteractionRange?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    } 
    
    private void UpdateInteraction(float distancePlayer)
    {
        distance = distancePlayer;
    }
    
    private void FixedUpdate()
    {
        if(playerTransform is null) return;
        if(!playerTransform.hasChanged) return;
        
        if(distance == 0) return;

        bool distanceByPlayer = Vector3.Distance(playerTransform.position, transform.position) > distance + radius;
        
        if (distanceByPlayer && !activeCanva || !distanceByPlayer && activeCanva)
        {
            return;
        }

        activeCanva=!activeCanva;
        
        uiInteraction.text = InputActionUtils.GetBestBindingString(playerActionMap.Player.Interaction);
        
        uiInteraction.transform.parent.gameObject.SetActive(activeCanva);
        
    }

    #endregion
    
}

public static class InputActionUtils
{
    public static string GetBestBindingString(InputAction action)
    {
        if (action.activeControl != null)
        {
            return InputControlPath.ToHumanReadableString(
                action.activeControl.path,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        }

        if (action.bindings.Count > 0)
        {
            return InputControlPath.ToHumanReadableString(
                action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        }

        return "N/A";
    }

    private static string GetCompositeKeys(InputAction action)
    {
        var keys = (from binding in action.bindings where binding.isPartOfComposite select InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice)).ToList();

        return string.Join("/", keys); // W/A/S/D
    }
    
    public static string ChangeTextForButton(string text = "", InputAction action = null)
    {
        if (text == "") return "";
        if (action == null) return text;
        
        foreach (var variable in action.bindings)
        {
            return text.Replace("<><>", variable.name is null ? GetBestBindingString(action) : GetCompositeKeys(action));
        }
        
        Debug.LogError("Dont have a input");
        return text;
    }
}
