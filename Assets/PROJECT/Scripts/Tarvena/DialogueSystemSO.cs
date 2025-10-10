using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSystemSO", menuName = "Texts/DialogueSystemSO")]
public class DialogueSystemSo : ScriptableObject
{
    [SerializeField]
    [Tooltip("Reference to the TextBoxes containing dialogue content in different languages")]
    private List<TextBoxesAndSprite> textBoxes;

    [SerializeField]
    [Tooltip("List of available dialogue choices")]
    private List<Choice> choice;
    
    public List<TextBoxesAndSprite> GetListTextBoxesSo { get => textBoxes; private set => textBoxes = value; }

    public TextBoxesAndSprite GetTextBoxesSo(int index) { return textBoxes[index]; }
    
    public List<Choice> GetChoice { get => choice; private set => choice = value; }
}

[Serializable]
public class Choice
{

    [field: Tooltip("The text that will be displayed as a dialogue choice")] 
    [SerializeField]
    private TextBoxesSo text;
    
    [field: Tooltip("The dialogue sequence that will be activated upon selecting this choice")] 
    [SerializeField]
    private DialogueSystemSo nextDialogueSystem;
    
    public TextBoxesSo GetText { get => text; private set => text = value; }
    
    public DialogueSystemSo GetNextDialogueSystem { get => nextDialogueSystem; private set => nextDialogueSystem = value; }
    
    public Choice(TextBoxesSo newText, DialogueSystemSo newNextDialogueSystem)
    {
        text = newText;
        nextDialogueSystem = newNextDialogueSystem;
    }
}

[Serializable]
public class TextBoxesAndSprite
{
    public TextBoxesSo text;
    public Sprite sprite;
}