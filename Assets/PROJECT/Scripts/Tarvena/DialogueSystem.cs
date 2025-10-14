using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MyEnum;

/// <summary>
/// Manages the dialogue system for displaying text, choices, and handling language localization.
/// Controls dialogue flow, user choices, and UI elements related to the dialogue system.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField, Tooltip("Scriptable object containing dialogue data")]
    private DialogueSystemSo dialogues;
    
    [SerializeField, Tooltip("Main dialogue text box UI element")] 
    private UITextButton dialogueBox;
    
    [SerializeField, Tooltip("List of choice text boxes for branching dialogue")]
    private List<UITextButton> textBoxesChoice = new();
    
    [SerializeField, Tooltip("Container for choice dialogue boxes")]
    private GameObject dialogueBoxChoice;

    [SerializeField, Tooltip("Character/scene artwork display")]
    private Image art;
    
    /// <summary>
    /// Event triggered when a dialogue sequence is completed
    /// </summary>
    public event Action<DialogueSystemSo> OnComplicitEvent;
    #endregion

    #region Private Fields
    private ConfigurationSo _config;
    private byte _index;
    private readonly List<TMP_Text> _textBoxesList = new();
    private bool _isChoice;
    #endregion

    #region Unity Lifecycle Methods
    private void OnEnable()
    {
        InitializeConfiguration();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion

    #region Private Methods
    private void InitializeConfiguration()
    {
        _config = GameConfig.Config;
        
        if (_config == null) return;
        
        _config.OnLanguageChanged += UpdateLanguage;
        if(dialogues is null) return;
        
        UpdateLanguage(_config.GetLanguage());
    }

    private void UnsubscribeFromEvents()
    {
        if (_config != null)
            _config.OnLanguageChanged -= UpdateLanguage;
    }

    /// <summary>
    /// Updates dialogue text based on the selected language
    /// </summary>
    /// <param name="lang">Language to display text in</param>
    private void UpdateLanguage(EnumLanguage lang)
    {
        if (_isChoice)
        {
            UpdateChoiceTexts(lang);
            return;
        }
        
        UpdateDialogueText(lang);
    }

    /// <summary>
    /// Updates text for all choice options in the selected language
    /// </summary>
    private void UpdateChoiceTexts(EnumLanguage lang)
    {
        for (var index = 0; index < _textBoxesList.Count; index++)
        {
            var newText = dialogues.Choices[index].Text.GetText(lang);
            _textBoxesList[index].text = newText;
        }
    }

    /// <summary>
    /// Updates main dialogue text in selected language
    /// </summary>
    private void UpdateDialogueText(EnumLanguage lang)
    {
        var newText = dialogues.GetDialogueContent(_index).text.GetText(lang);

        if (newText == null)
        {
            LogDialogueError(null);
            return;
        }

        if (_textBoxesList.Count == 0)
        {
            InitializeFirstDialogue(newText);
            return;
        }
        
        _textBoxesList[0].text = newText;
    }

    private void LogDialogueError(string newText)
    {
        Debug.LogError($"Text retrieval error: text={newText}, index={_index}, dialogues={dialogues}, " +
                      $"current dialogue={dialogues.GetDialogueContent(_index)}, total count:{dialogues.DialogueContents.Count}");
    }

    private void InitializeFirstDialogue(string newText)
    {
        art.sprite = dialogues.GetDialogueContent(0).sprite;
        dialogueBox.buttonText.text = newText;
    }
    
    private void CompletedDialogue(DialogueSystemSo obj)
    {
        OnComplicitEvent?.Invoke(obj);
        dialogues = null;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Advances to the next dialogue segment or shows choices if available
    /// </summary>
    public void NextDialogue()
    {
        _textBoxesList.Clear();
        _index++;

        if (dialogues.DialogueContents.Count > _index)
        {
            ContinueDialogue();
        }
        else
        {
            HandleDialogueEnd();
        }

        UpdateLanguage(_config.GetLanguage());
    }
    
    /// <summary>
    /// Handles selection of a dialogue choice
    /// </summary>
    /// <param name="value">Index of selected choice</param>
    public void Choice(int value)
    {
        var nextDialogueSystem = dialogues.Choices[value].NextDialogue;

        if (nextDialogueSystem == null)
        {
            CompletedDialogue(dialogues);
            gameObject.SetActive(false);
            return;
        }
        
        NewDialogue(nextDialogueSystem);
    }
    
    /// <summary>
    /// Starts a new dialogue sequence
    /// </summary>
    /// <param name="dialogueSystemSo">New dialogue system to use</param>
    public Task NewDialogue(DialogueSystemSo dialogueSystemSo)
    {
        if (dialogues is not null && dialogues != dialogueSystemSo)
        {
            CompletedDialogue(dialogues);
        }
        
        ResetDialogueState();
        dialogues = dialogueSystemSo;
        SetupDialogueUI();
        UpdateLanguage(_config.GetLanguage());
        
        return Task.CompletedTask;
    }
    #endregion

    #region Helper Methods
    private void ContinueDialogue()
    {
        _textBoxesList.Add(dialogueBox.buttonText);
        art.sprite = dialogues.GetDialogueContent(_index).sprite;
    }

    private void HandleDialogueEnd()
    {
        if (dialogues.Choices.Count == 0)
        {
            CompletedDialogue(dialogues);
            gameObject.SetActive(false);
            return;
        }
        
        ShowChoices();
    }

    private void ShowChoices()
    {
        _isChoice = true;
        dialogueBoxChoice.SetActive(true);
        dialogueBox.gameObject.SetActive(false);
        
        foreach (var t in textBoxesChoice)
        {
            _textBoxesList.Add(t.buttonText);
            t.gameObject.SetActive(true);
        }
    }

    private void ResetDialogueState()
    {
        _isChoice = false;
        _index = 0;
        _textBoxesList.Clear();
    }

    private void SetupDialogueUI()
    {
        gameObject.SetActive(true);
        dialogueBox.gameObject.SetActive(true);
        dialogueBoxChoice.SetActive(false);
    }
    #endregion
}