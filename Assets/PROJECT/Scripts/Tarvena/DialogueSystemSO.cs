using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that manages dialogue systems with multiple language support and choice-based conversations.
/// </summary>
[CreateAssetMenu(fileName = "DialogueSystemSO", menuName = "Dialogue/DialogueSystem")]
public class DialogueSystemSo : ScriptableObject
{
    [SerializeField]
    [Tooltip("Collection of text boxes and associated sprites for different dialogue states")]
    private List<DialogueContent> dialogueContents = new();

    [SerializeField]
    [Tooltip("Available dialogue choices and their consequences")]
    private List<DialogueChoice> choices = new();

    // Properties with validation
    public IReadOnlyList<DialogueContent> DialogueContents => dialogueContents;
    public IReadOnlyList<DialogueChoice> Choices => choices;

    /// <summary>
    /// Retrieves specific dialogue content by index with bound checking
    /// </summary>
    public DialogueContent GetDialogueContent(int index)
    {
        if (index < 0 || index >= dialogueContents.Count)
        {
            Debug.LogError($"Invalid dialogue content index: {index}");
            return null;
        }
        return dialogueContents[index];
    }
}

/// <summary>
/// Represents a dialogue choice with associated text and next dialogue sequence
/// </summary>
[Serializable]
public class DialogueChoice
{
    [SerializeField]
    [Tooltip("Text to display for this choice")]
    private TextBoxesSo choiceText;

    [SerializeField]
    [Tooltip("Next dialogue sequence after selecting this choice")]
    private DialogueSystemSo nextDialogue;

    // Immutable properties
    public TextBoxesSo Text => choiceText;
    public DialogueSystemSo NextDialogue => nextDialogue;

    public DialogueChoice(TextBoxesSo text, DialogueSystemSo nextDialogue)
    {
        this.choiceText = text ?? throw new ArgumentNullException(nameof(text));
        this.nextDialogue = nextDialogue;
    }
}

/// <summary>
/// Contains the text and visual elements for a dialogue entry
/// </summary>
[Serializable]
public class DialogueContent
{
    [SerializeField]
    [Tooltip("Text content in multiple languages")]
    public TextBoxesSo text;

    [SerializeField]
    [Tooltip("Associated sprite/image for this dialogue")]
    public Sprite sprite;

    public DialogueContent(TextBoxesSo text, Sprite sprite)
    {
        this.text = text ?? throw new ArgumentNullException(nameof(text));
        this.sprite = sprite;
    }
}