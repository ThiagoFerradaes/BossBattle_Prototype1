using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITextButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool chose;
    [SerializeField] private byte ID;
    
    [SerializeField] private DialogueSystem dialogue;
    
    [SerializeField] private Color clickedColor, normalColor, hoverColor;
    
    public Image image;
    
    public TMP_Text text;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        image.color = clickedColor;
        Invoke(nameof(NormalizedColor), 0.3f);

        if (!chose)
        {
            dialogue.NextDialogue();
            return;
        }
        dialogue.Choice(ID);
    }

    private void NormalizedColor()
    {
        image.color = normalColor;
    }
}
