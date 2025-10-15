using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages UI button behavior with text and color state changes based on user interaction.
/// Handles dialogue system integration for both regular progression and choice selection.
/// </summary>
[RequireComponent(typeof(Image))]
public class UITextButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [SerializeField]
    [Tooltip("Determines if this button represents a dialogue choice")]
    private bool isChoice;

    [SerializeField]
    [Tooltip("Unique identifier for choice selection")]
    private byte choiceId;
    
    [SerializeField]
    [Tooltip("Reference to the dialogue system controller")]
    private DialogueSystem dialogueSystem;
    
    [Header("Button Colors")]
    [SerializeField]
    [Tooltip("Color when button is clicked")]
    private Color clickedColor = Color.gray;
    
    [SerializeField]
    [Tooltip("Default button color")]
    private Color normalColor = Color.white;
    
    [SerializeField]
    [Tooltip("Color when mouse hovers over button")]
    private Color hoverColor = Color.yellow;
    #endregion

    #region Public References
    [Header("UI Components")]
    public Image buttonImage;
    public TMP_Text buttonText;
    #endregion

    #region Constants
    private const float ColorTransitionDelay = 0.3f;
    #endregion

    #region Unity Event Handlers
    private void Awake()
    {
        ValidateReferences();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = normalColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        HandleButtonClick();
    }
    #endregion

    #region Private Methods
    private void ValidateReferences()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        if (dialogueSystem == null)
        {
            Debug.LogError($"DialogueSystem reference missing on {gameObject.name}");
        }
    }

    private void HandleButtonClick()
    {
        buttonImage.color = clickedColor;
        Invoke(nameof(ResetButtonColor), ColorTransitionDelay);

        if (!isChoice)
        {
            dialogueSystem.NextDialogue();
            return;
        }
        
        dialogueSystem.Choice(choiceId);
    }

    private void ResetButtonColor()
    {
        buttonImage.color = normalColor;
    }
    #endregion
}