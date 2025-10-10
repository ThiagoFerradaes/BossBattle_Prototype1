using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that manages the dialogue system configuration.
/// Handles text content, character sprites, and dialogue choices.
/// </summary>
[CreateAssetMenu(fileName = "DialogueSystemSO", menuName = "Texts/DialogueSystemSO")]
public class DialogueSystemSo : ScriptableObject
{
    [SerializeField]
    [Tooltip("Reference to the TextBoxes containing dialogue content in different languages")]
    private List<TextBoxesSo> textBoxes;
    
    [SerializeField]
    [Tooltip("Character sprite displayed during the dialogue")]
    private Sprite sprite;

    [SerializeField]
    [Tooltip("List of available dialogue choices")]
    private List<Choice> choice;
    
    /// <summary>
    /// Gets the TextBoxes containing dialogue content
    /// </summary>
    public List<TextBoxesSo> GetListTextBoxesSo { get => textBoxes; private set => textBoxes = value; }

    public TextBoxesSo GetTextBoxesSo(int index) { return textBoxes[index]; }
    
    /// <summary>
    /// Gets the character sprite for the dialogue
    /// </summary>
    public Sprite GetSprite { get => sprite; private set => sprite = value; }

    /// <summary>
    /// Gets the list of available dialogue choices
    /// </summary>
    public List<Choice> GetChoice { get => choice; private set => choice = value; }
}
/// <summary>
/// Represents a dialogue choice option that contains text content and links to subsequent dialogue sequences.
/// </summary>
[Serializable]
public class Choice
{
    /// <summary>
    /// The text content displayed to the player as a selectable dialogue option.
    /// </summary>
    //[field: TextArea(0, 10)] 
    [field: Tooltip("The text that will be displayed as a dialogue choice")] 
    [SerializeField]
    private TextBoxesSo text;

    /// <summary>
    /// Reference to the dialogue sequence that will be triggered when this choice is selected.
    /// </summary>
    [field: Tooltip("The dialogue sequence that will be activated upon selecting this choice")] 
    [SerializeField]
    private DialogueSystemSo nextDialogueSystem;

    /// <summary>
    /// Gets or sets the text content of the dialogue choice.
    /// </summary>
    public TextBoxesSo GetText { get => text; private set => text = value; }

    /// <summary>
    /// Gets or sets the next dialogue sequence associated with this choice.
    /// </summary>
    public DialogueSystemSo GetNextDialogueSystem { get => nextDialogueSystem; private set => nextDialogueSystem = value; }
    
    /// <summary>
    /// Creates a new dialogue choice with specified text and later dialogue sequence.
    /// </summary>
    /// <param name="newText">The text content to be displayed for this choice</param>
    /// <param name="newNextDialogueSystem">The dialogue sequence to be triggered when this choice is selected</param>
    public Choice(TextBoxesSo newText, DialogueSystemSo newNextDialogueSystem)
    {
        text = newText;
        nextDialogueSystem = newNextDialogueSystem;
    }
}